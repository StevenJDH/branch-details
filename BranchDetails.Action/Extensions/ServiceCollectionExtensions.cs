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
using GitHub;
using GitHub.Octokit.Authentication;
using GitHub.Octokit.Client;
using Microsoft.Extensions.DependencyInjection;

namespace BranchDetails.Action.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitHubActionServices(this IServiceCollection services)
    {
        services.AddSingleton<Context>();
        services.AddSingleton<ActionInputs>();

        services.AddSingleton(_ =>
        {
            var provider = services.BuildServiceProvider();
            var inputs = provider.GetRequiredService<ActionInputs>();
            string gitHubToken = inputs.GitHubToken;
            ArgumentException.ThrowIfNullOrWhiteSpace(gitHubToken);
            var request = RequestAdapter.Create(new TokenAuthenticationProvider("Octokit.Gen", gitHubToken));
            return new GitHubClient(request);
        });

        services.AddSingleton<IGitHubService, GitHubService>();
        services.AddSingleton<BranchDetailsProcessor>();

        return services;
    }
}