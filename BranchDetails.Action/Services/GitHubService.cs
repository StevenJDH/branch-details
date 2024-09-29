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
using GitHub;
using GitHub.Models;

namespace BranchDetails.Action.Services;

internal class GitHubService(GitHubClient github, Context context) : IGitHubService
{
    private readonly GitHubClient _github = github;
    private readonly string _repoOwner = context.RepositoryOwner;
    private readonly string _repo = context.Repository.Split("/")[^1];

    /// <inheritdoc />
    public async ValueTask<string?> GetTaggedBranchAsync(string? tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);

        var gitRef = await GetTagRefAsync(tag);

        if (gitRef is {Ref: not null} && gitRef.Ref.StartsWith("refs/heads/"))
        {
            return gitRef.Ref.Split("/")[^1];
        }

        return await GetDefaultBranchAsync();
    }

    private async ValueTask<GitRef?> GetTagRefAsync(string tag)
    {
        var gitRefs = await _github.Repos[_repoOwner][_repo].Git.MatchingRefs[tag].GetAsync();
        return gitRefs?.Find(i => i.Ref?.EndsWith(tag) ?? false);
    }

    public async ValueTask<string?> GetDefaultBranchAsync()
    {
        var response = await _github.Repos[_repoOwner][_repo].GetAsync();
        return response?.DefaultBranch;
    }
}