/*
 * This file is part of Branch Details <https://github.com/StevenJDH/branch-details>.
 * Copyright (C) 2024 Steven Jenkins De Haro.
 *
 * Branch Details is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Branch Details is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Branch Details.  If not, see <http://www.gnu.org/licenses/>.
 */

using BranchDetails.Action.Data;
using BranchDetails.Action.Services;
using BranchDetails.Action.Tests.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using static BranchDetails.Action.Data.Core;

namespace BranchDetails.Action.Tests;

[TestFixture]
public class BranchDetailsProcessorTests
{
    private readonly string _outputFile = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");
    private readonly string _envFile = Path.Combine(Directory.GetCurrentDirectory(), "env.txt");
    private readonly string _summaryFile = Path.Combine(Directory.GetCurrentDirectory(), "summary.txt");
    // ReSharper disable once InconsistentNaming
    private const string EXPECTED_START_PROCESSING_MESSAGE = "💾 Processing branch details for run id 123456...";
    // ReSharper disable once InconsistentNaming
    private const string EXPECTED_DONE_PROCESSING_MESSAGE = "🎉 Process completed successfully.";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("GITHUB_RUN_ID", "123456");
        Environment.SetEnvironmentVariable("GITHUB_OUTPUT", _outputFile);
        Environment.SetEnvironmentVariable("GITHUB_ENV", _envFile);
        Environment.SetEnvironmentVariable("GITHUB_STEP_SUMMARY", _summaryFile);
    }

    [SetUp]
    public void SetUp()
    {
        File.Create(_outputFile).Dispose();
        File.Create(_envFile).Dispose();
        File.Create(_summaryFile).Dispose();
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("INPUT_EXPORT-VARIABLES", null);
        Environment.SetEnvironmentVariable("INPUT_DROP-TAG-PREFIX", null);
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", null);
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", null);
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", null);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", null);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", null);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", null);
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", null);
        Environment.SetEnvironmentVariable("GITHUB_REF", null);
        File.Delete(_summaryFile);
        File.Delete(_envFile);
        File.Delete(_outputFile);
    }

    [Test, Description("Should set push specific values when committing to branch.")]
    public async Task Should_SetPushSpecificValues_When_CommittingToBranch()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/heads/test");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "1");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var mockLogger = new Mock<ILogger<BranchDetailsProcessor>>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(mockLogger.Object, ctx, inputs, mockGitHubService.Object);

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .ReturnsAsync("test");

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Success));
            Assert.That(outputPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs, Has.Count.EqualTo(0));
            Assert.That(outputPairs.GetValueOrDefault("triggering_ref"), Is.EqualTo("refs/heads/test"));
            Assert.That(outputPairs.GetValueOrDefault("is_tag"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_semver"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("tag"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("current_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("is_default_branch"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("default_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("base_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("head_branch_name"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("is_pull_request"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("pull_request_id"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("repo_owner_name"), Is.EqualTo("stevenjdh"));
            Assert.That(outputPairs.GetValueOrDefault("repo_name"), Is.EqualTo("branch-details"));
            Assert.That(outputPairs.GetValueOrDefault("event_actor_name"), Is.EqualTo("stevenjdh"));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_rerun"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_owner"), Is.EqualTo(bool.TrueString.ToLower()));
        });
        mockLogger.VerifyLog(LogLevel.Information, Times.Once, EXPECTED_START_PROCESSING_MESSAGE);
        mockLogger.VerifyLog(LogLevel.Information, Times.Once, EXPECTED_DONE_PROCESSING_MESSAGE);
    }

    [Test, Description("Should set PR specific values when trigger is a pull request.")]
    public async Task Should_SetPrSpecificValues_When_TriggerIsAPullRequest()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/pull/123/merge");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", "test");
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", "feature/example");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "1");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var logger = new NullLogger<BranchDetailsProcessor>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(logger, ctx, inputs, mockGitHubService.Object);

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .ReturnsAsync("test");

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Success));
            Assert.That(outputPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs, Has.Count.EqualTo(0));
            Assert.That(outputPairs.GetValueOrDefault("triggering_ref"), Is.EqualTo("refs/pull/123/merge"));
            Assert.That(outputPairs.GetValueOrDefault("is_tag"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_semver"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("tag"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("current_branch_name"), Is.EqualTo("feature/example"));
            Assert.That(outputPairs.GetValueOrDefault("is_default_branch"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("default_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("base_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("head_branch_name"), Is.EqualTo("feature/example"));
            Assert.That(outputPairs.GetValueOrDefault("is_pull_request"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("pull_request_id"), Is.EqualTo("123"));
            Assert.That(outputPairs.GetValueOrDefault("repo_owner_name"), Is.EqualTo("stevenjdh"));
            Assert.That(outputPairs.GetValueOrDefault("repo_name"), Is.EqualTo("branch-details"));
            Assert.That(outputPairs.GetValueOrDefault("event_actor_name"), Is.EqualTo("stevenjdh"));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_rerun"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_owner"), Is.EqualTo(bool.TrueString.ToLower()));
        });
    }

    [Test, Description("Should set tag specific values when trigger is a tag.")]
    public async Task Should_SetTagSpecificValues_When_TriggerIsATag()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/tags/v1.0.0");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "BuildBot");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "2");
        Environment.SetEnvironmentVariable("INPUT_DROP-TAG-PREFIX", "v");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var logger = new NullLogger<BranchDetailsProcessor>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(logger, ctx, inputs, mockGitHubService.Object);

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .ReturnsAsync("test");

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Success));
            Assert.That(outputPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs, Has.Count.EqualTo(0));
            Assert.That(outputPairs.GetValueOrDefault("triggering_ref"), Is.EqualTo("refs/tags/v1.0.0"));
            Assert.That(outputPairs.GetValueOrDefault("is_tag"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_semver"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("tag"), Is.EqualTo("1.0.0"));
            Assert.That(outputPairs.GetValueOrDefault("current_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("is_default_branch"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("default_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("base_branch_name"), Is.EqualTo("test"));
            Assert.That(outputPairs.GetValueOrDefault("head_branch_name"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("is_pull_request"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("pull_request_id"), Is.EqualTo(string.Empty));
            Assert.That(outputPairs.GetValueOrDefault("repo_owner_name"), Is.EqualTo("stevenjdh"));
            Assert.That(outputPairs.GetValueOrDefault("repo_name"), Is.EqualTo("branch-details"));
            Assert.That(outputPairs.GetValueOrDefault("event_actor_name"), Is.EqualTo("buildbot"));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_rerun"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(outputPairs.GetValueOrDefault("is_event_actor_owner"), Is.EqualTo(bool.FalseString.ToLower()));
        });
    }

    [Test, Description("Should export environment variables when enabled from inputs.")]
    public async Task Should_ExportEnvironmentVariables_When_EnabledFromInputs()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/heads/test");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "1");
        Environment.SetEnvironmentVariable("INPUT_EXPORT-VARIABLES", "true");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var logger = new NullLogger<BranchDetailsProcessor>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(logger, ctx, inputs, mockGitHubService.Object);

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .ReturnsAsync("test");

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Success));
            Assert.That(outputPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs.GetValueOrDefault("BD_TRIGGERING_REF"), Is.EqualTo("refs/heads/test"));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_TAG"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_SEMVER"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(envPairs.GetValueOrDefault("BD_TAG"), Is.EqualTo(string.Empty));
            Assert.That(envPairs.GetValueOrDefault("BD_CURRENT_BRANCH_NAME"), Is.EqualTo("test"));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_DEFAULT_BRANCH"), Is.EqualTo(bool.TrueString.ToLower()));
            Assert.That(envPairs.GetValueOrDefault("BD_DEFAULT_BRANCH_NAME"), Is.EqualTo("test"));
            Assert.That(envPairs.GetValueOrDefault("BD_BASE_BRANCH_NAME"), Is.EqualTo("test"));
            Assert.That(envPairs.GetValueOrDefault("BD_HEAD_BRANCH_NAME"), Is.EqualTo(string.Empty));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_PULL_REQUEST"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(envPairs.GetValueOrDefault("BD_PULL_REQUEST_ID"), Is.EqualTo(string.Empty));
            Assert.That(envPairs.GetValueOrDefault("BD_REPO_OWNER_NAME"), Is.EqualTo("stevenjdh"));
            Assert.That(envPairs.GetValueOrDefault("BD_REPO_NAME"), Is.EqualTo("branch-details"));
            Assert.That(envPairs.GetValueOrDefault("BD_EVENT_ACTOR_NAME"), Is.EqualTo("stevenjdh"));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_EVENT_ACTOR_RERUN"), Is.EqualTo(bool.FalseString.ToLower()));
            Assert.That(envPairs.GetValueOrDefault("BD_IS_EVENT_ACTOR_OWNER"), Is.EqualTo(bool.TrueString.ToLower()));
        });
    }

    [Test, Description("Should generate step summary for all action outputs.")]
    public async Task Should_GenerateStepSummary_ForAllActionOutputs()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/heads/test");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "1");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var logger = new NullLogger<BranchDetailsProcessor>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(logger, ctx, inputs, mockGitHubService.Object);
        string expectedSummary = """
            ### Branch Details GitHub Action

            Below is a list of all the generated outputs for this run.

            | Name | Value |
            | --- | --- |
            | triggering_ref | refs/heads/test |
            | is_tag | false |
            | is_semver | false |
            | tag |  |
            | current_branch_name | test |
            | is_default_branch | true |
            | default_branch_name | test |
            | base_branch_name | test |
            | head_branch_name |  |
            | is_pull_request | false |
            | pull_request_id |  |
            | repo_owner_name | stevenjdh |
            | repo_name | branch-details |
            | event_actor_name | stevenjdh |
            | is_event_actor_rerun | false |
            | is_event_actor_owner | true |

            """;

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .ReturnsAsync("test");

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);
        string summary = await File.ReadAllTextAsync(_summaryFile);


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Success));
            Assert.That(outputPairs, Has.Count.EqualTo(16));
            Assert.That(envPairs, Has.Count.EqualTo(0));
            Assert.That(summary, Is.EqualTo(expectedSummary));
        });
    }

    [Test, Description("Should return failure status when an exception is thrown.")]
    public async Task Should_ReturnFailureStatus_When_AnExceptionIsThrown()
    {
        // Arrange
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/tags/ "); // Whitespace is intentional.
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_HEAD_REF", string.Empty);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_RUN_ATTEMPT", "1");
        Environment.SetEnvironmentVariable("INPUT_DROP-TAG-PREFIX", "v");

        var ctx = new Context();
        var inputs = new ActionInputs();
        var logger = new InMemoryLogger<BranchDetailsProcessor>();
        var mockGitHubService = new Mock<IGitHubService>();
        var processor = new BranchDetailsProcessor(logger, ctx, inputs, mockGitHubService.Object);
        string expectedErrorMessage = "This is a test error message.";

        mockGitHubService.Setup(x => x.GetDefaultBranchAsync())
            .Throws(new ArgumentException(expectedErrorMessage));

        // Act
        var exitCode = await processor.StartProcessingBranchAsync();
        var outputPairs = LoadPairs(_outputFile);
        var envPairs = LoadPairs(_envFile);
        string summary = await File.ReadAllTextAsync(_summaryFile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(ExitCode.Failure));
            Assert.That(outputPairs, Has.Count.EqualTo(0));
            Assert.That(envPairs, Has.Count.EqualTo(0));
            Assert.That(summary, Is.EqualTo(string.Empty));
            Assert.That(logger.GetLogMessages(), Has.Count.EqualTo(2));
            Assert.That(logger.GetLogMessages(), Contains.Item((LogLevel.Information, EXPECTED_START_PROCESSING_MESSAGE)));
            Assert.That(logger.GetLogMessages(), Contains.Item((LogLevel.Error, expectedErrorMessage)));
        });
    }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static IReadOnlyDictionary<string, string> LoadPairs(string path)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
    {
        var pairs = new Dictionary<string, string>();
        using var reader = new StreamReader(path);

        while (reader.ReadLine() is { } line)
        {
            string[] parts = line.Split('=');

            if (parts.Length is not 2)
            {
                continue;
            }

            string key = parts[0].Trim();
            string val = parts[1].Trim();

            pairs[key] = val;
        }

        return pairs;
    }
}