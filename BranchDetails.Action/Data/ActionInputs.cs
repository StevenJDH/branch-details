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

namespace BranchDetails.Action.Data;

public class ActionInputs
{
    /// <summary>
    /// Removes the prefix of a tag. For example, the v in version v1.0.0 tag.
    /// </summary>
    public string DropTagPrefix => Core.GetInput("drop-tag-prefix", new(Required: false));

    /// <summary>
    /// Indicates whether or not to set environment variables for each output exposed by this action for
    /// other steps in a job.
    /// </summary>
    public string ExportVariables => Core.GetInput("export-variables", new(Required: false));

    /// <summary>
    /// The GitHub token to authenticate API requests. Assigned from <code>${{ secrets.GITHUB_TOKEN }}</code>.
    /// </summary>
    public string GitHubToken => Core.GetInput("github-token");
}