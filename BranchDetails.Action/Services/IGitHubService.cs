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

namespace BranchDetails.Action.Services;

internal interface IGitHubService
{
    /// <summary>
    /// Tags on commits aren't associated to branches, so their references are refs/tags/[tag] and we can assume default branch. However,
    /// if the tag is on a commit that is associated with a branch, their references will be refs/heads/[branch].
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    ValueTask<string?> GetTaggedBranchAsync(string? tag);

    ValueTask<string?> GetDefaultBranchAsync();
}