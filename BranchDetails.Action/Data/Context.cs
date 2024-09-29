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

// Ignore Spelling: Env

namespace BranchDetails.Action.Data;

public sealed class Context
{
    /// <summary>
    /// The name of the person or app that initiated the workflow. For example, StevenJDH.
    /// </summary>
    public string Actor { get; init; }

    /// <summary>
    /// The username of the user that initiated the workflow run. If the workflow run is a re-run, this value may
    /// differ from <see cref="Actor"/>. Any workflow re-runs will use the privileges of <see cref="Actor"/>, even if the actor
    /// initiating the re-run (<see cref="TriggeringActor"/>) has different privileges.
    /// </summary>
    public string TriggeringActor { get; init; }

    /// <summary>
    /// A unique number for each attempt of a particular workflow run in a repository. This number begins at 1
    /// for the workflow run's first attempt, and increments with each re-run.
    /// </summary>
    public int RunAttempt { get; init; }

    /// <summary>
    /// A unique number for each workflow run within a repository. This number does not change if you re-run the
    /// workflow run. For example, 1658821493.
    /// </summary>
    public long RunId { get; init; }

    /// <summary>
    /// The fully-formed ref of the branch or tag that triggered the workflow run. For workflows triggered by
    /// push, this is the branch or tag ref that was pushed. For workflows triggered by pull_request, this is
    /// the pull request merge branch. For workflows triggered by release, this is the release tag created. For
    /// other triggers, this is the branch or tag ref that triggered the workflow run. This is only set if a
    /// branch or tag is available for the event type. The ref given is fully-formed, meaning that for branches
    /// the format is refs/heads/[branch_name], for pull requests it is refs/pull/[pr_number]/merge, and for
    /// tags it is refs/tags/[tag_name]. For example, refs/heads/feature-branch-1.
    /// </summary>
    public string Ref { get; init; }

    /// <summary>
    /// The name of the base ref or target branch of the pull request in a workflow run. This is only set when
    /// the event that triggers a workflow run is either pull_request or pull_request_target. For example, main.
    /// </summary>
    public string? BaseRef { get; init; }

    /// <summary>
    /// The head ref or source branch of the pull request in a workflow run. This property is only set when the
    /// event that triggers a workflow run is either pull_request or pull_request_target. For example,
    /// feature-branch-1.
    /// </summary>
    public string? HeadRef { get; init; }

    /// <summary>
    /// The path on the runner to the file that sets the current step's outputs from workflow commands. This
    /// file is unique to the current step and changes for each step in a job. For example,
    /// /home/runner/work/_temp/_runner_file_commands/set_output_a50ef383-b063-46d9-9157-57953fc9f3f0.
    /// </summary>
    public string OutputFile { get; init; }

    /// <summary>
    /// The path on the runner to the file that sets variables from workflow commands. This file is unique
    /// to the current step and changes for each step in a job. For example,
    /// /home/runner/work/_temp/_runner_file_commands/set_env_87406d6e-4979-4d42-98e1-3dab1f48b13a.
    /// </summary>
    public string EnvFile { get; init; }

    /// <summary>
    /// The owner and repository name. For example, StevenJDH/branch-details.
    /// </summary>
    public string Repository { get; init; }

    /// <summary>
    /// The repository owner's name. For example, StevenJDH.
    /// </summary>
    public string RepositoryOwner { get; init; }

    /// <summary>
    /// The default working directory on the runner for steps, and the default location of your repository
    /// when using the checkout action. For example, /home/runner/work/my-repo-name/my-repo-name.
    /// </summary>
    public string WorkspaceDir { get; init; }

    /// <summary>
    /// The operating system of the runner executing the job. Possible values are Linux, Windows, or macOS.
    /// For example, Windows.
    /// </summary>
    public string RunnerOs { get; init; }

    /// <summary>
    /// The path to a temporary directory on the runner. This directory is emptied at the beginning and end
    /// of each job. Note that files will not be removed if the runner's user account does not have
    /// permission to delete them. For example, D:\a\_temp.
    /// </summary>
    public string RunnerTempDir { get; init; }

    /// <summary>
    /// The path on the runner to the file that contains job summaries from workflow commands. This file is
    /// unique to the current step and changes for each step in a job. For example,
    /// /home/runner/_layout/_work/_temp/_runner_file_commands/step_summary_1cb22d7f-5663-41a8-9ffc-13472605c76c.
    /// </summary>
    public string StepSummaryFile { get; init; }

    public Context()
    {
        var target = EnvironmentVariableTarget.Process;
        
        Actor = Environment.GetEnvironmentVariable("GITHUB_ACTOR", target)!;
        TriggeringActor = Environment.GetEnvironmentVariable("GITHUB_TRIGGERING_ACTOR", target)!;
        RunAttempt = int.TryParse(Environment.GetEnvironmentVariable("GITHUB_RUN_ATTEMPT", target), out var attempt) ? attempt : -1;
        RunId = long.TryParse(Environment.GetEnvironmentVariable("GITHUB_RUN_ID", target), out var id) ? id : -1;
        Ref = Environment.GetEnvironmentVariable("GITHUB_REF", target)!;
        BaseRef = Environment.GetEnvironmentVariable("GITHUB_BASE_REF", target);
        HeadRef = Environment.GetEnvironmentVariable("GITHUB_HEAD_REF", target);
        OutputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT", target)!;
        EnvFile = Environment.GetEnvironmentVariable("GITHUB_ENV", target)!;
        Repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY", target)!;
        RepositoryOwner = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", target)!;
        WorkspaceDir = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE", target)!;
        RunnerOs = Environment.GetEnvironmentVariable("RUNNER_OS", target)!;
        RunnerTempDir = Environment.GetEnvironmentVariable("RUNNER_TEMP", target)!;
        StepSummaryFile = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY", target)!;
    }
}