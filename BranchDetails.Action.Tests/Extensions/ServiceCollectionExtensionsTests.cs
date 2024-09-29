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

using BranchDetails.Action.Extensions;
using Microsoft.Extensions.DependencyInjection;
using BranchDetails.Action.Data;
using BranchDetails.Action.Services;
using GitHub;

namespace BranchDetails.Action.Tests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private readonly string _outputFile = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");

    [OneTimeSetUp]
    public void SetUp()
    {
        File.Create(_outputFile).Dispose();
        Environment.SetEnvironmentVariable("GITHUB_REF", "refs/heads/test");
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", "refs/heads/test");
        Environment.SetEnvironmentVariable("GITHUB_OUTPUT", _outputFile);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", "StevenJDH");
        Environment.SetEnvironmentVariable("INPUT_GITHUB-TOKEN", "123456");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("INPUT_GITHUB-TOKEN", null);
        Environment.SetEnvironmentVariable("GITHUB_ACTOR", null);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", null);
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", null);
        Environment.SetEnvironmentVariable("GITHUB_OUTPUT", null);
        Environment.SetEnvironmentVariable("GITHUB_BASE_REF", null);
        Environment.SetEnvironmentVariable("GITHUB_REF", null);
        File.Delete(_outputFile);
    }

    [Test, Description("Should add GitHub Context for dependency injection.")]
    public void Should_AddGitHubContext_ForDependencyInjection()
    {
        var services = new ServiceCollection();
        var provider = services
            .AddGitHubActionServices()
            .BuildServiceProvider();
        var ctx = provider.GetService<Context>();

        Assert.That(ctx, Is.Not.Null);
        Assert.That(ctx, Is.InstanceOf<Context>());
    }

    [Test, Description("Should add action inputs for dependency injection.")]
    public void Should_AddActionInputs_ForDependencyInjection()
    {
        var services = new ServiceCollection();
        var provider = services
            .AddGitHubActionServices()
            .BuildServiceProvider();
        var inputs = provider.GetService<ActionInputs>();

        Assert.That(inputs, Is.Not.Null);
        Assert.That(inputs, Is.InstanceOf<ActionInputs>());
    }

    [Test, Description("Should add GitHub client for dependency injection.")]
    public void Should_AddGitHubClient_ForDependencyInjection()
    {
        var services = new ServiceCollection();
        var provider = services
            .AddGitHubActionServices()
            .BuildServiceProvider();
        var client = provider.GetService<GitHubClient>();

        Assert.That(client, Is.Not.Null);
        Assert.That(client, Is.InstanceOf<GitHubClient>());
    }

    [Test, Description("Should add GitHub Service for dependency injection.")]
    public void Should_AddGitHubService_ForDependencyInjection()
    {
        var services = new ServiceCollection();
        var provider = services
            .AddGitHubActionServices()
            .BuildServiceProvider();
        var github = provider.GetService<IGitHubService>();

        Assert.That(github, Is.Not.Null);
        Assert.That(github, Is.InstanceOf<IGitHubService>());
    }

    [Test, Description("Should add branch name processor for dependency injection.")]
    public void Should_AddBranchDetailsProcessor_ForDependencyInjectionAsync()
    {
        var services = new ServiceCollection();
        var provider = services
            .AddLogging()
            .AddGitHubActionServices()
            .BuildServiceProvider();
        var processor = provider.GetService<BranchDetailsProcessor>();

        Assert.That(processor, Is.Not.Null);
        Assert.That(processor, Is.InstanceOf<BranchDetailsProcessor>());
    }
}