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

using System.Text.RegularExpressions;
using BranchDetails.Action.Data;
using BranchDetails.Action.Services;
using BranchDetails.Action.Validators;
using Microsoft.Extensions.Logging;
using static BranchDetails.Action.Data.Core;

namespace BranchDetails.Action;

internal sealed class BranchDetailsProcessor(ILogger<BranchDetailsProcessor> logger, Context context, ActionInputs inputs, IGitHubService github)
{
    private readonly ILogger<BranchDetailsProcessor> _logger = logger;
    private readonly Context _context = context;
    private readonly ActionInputs _inputs = inputs;
    private readonly IGitHubService _github = github;

    public async ValueTask<ExitCode> StartProcessingBranchAsync()
    {
        try
        {
            _logger.LogInformation("💾 Processing branch details for run id {RunId}...", _context.RunId);

            bool isPR = _context.Ref.StartsWith("refs/pull/");
            bool isTag = _context.Ref.StartsWith("refs/tags/");
            bool isSemVer = false;
            string? tag = null;
            string? targetBranchName;
            string? sourceBranchName = null;
            string? currentBranchName;
            string? pullRequestId = null;

            if (isTag)
            {
                string pattern = @$"^{_inputs.DropTagPrefix}";
                string originalTag = _context.Ref.Split("/")[^1];
                tag = Regex.Replace(originalTag, pattern, "");
                isSemVer = SemVerValidator.IsValid(tag);
                targetBranchName = await _github.GetTaggedBranchAsync(originalTag);
                currentBranchName = targetBranchName;
            }
            else if (isPR)
            {
                targetBranchName = _context.BaseRef;
                sourceBranchName = _context.HeadRef;
                currentBranchName = sourceBranchName;
                pullRequestId = _context.Ref.Split("/")[^2];
            }
            else
            {
                targetBranchName = _context.Ref.Split("/")[^1];
                currentBranchName = targetBranchName;
            }

            string? defaultBranchName = await _github.GetDefaultBranchAsync();
            bool isDefaultBranch = currentBranchName?.Equals(defaultBranchName, StringComparison.InvariantCultureIgnoreCase) ?? false;
            string owner = _context.RepositoryOwner;
            bool isRerun = _context.RunAttempt > 1;
            string eventActor = isRerun ? _context.TriggeringActor.ToLower() : _context.Actor.ToLower();

            IReadOnlyList<string> columns = ["Name", "Value"];

            IReadOnlyList<IReadOnlyList<string>> rows =
            [
                await SetOutputAsync("triggering_ref", _context.Ref),
                await SetOutputAsync("is_tag", isTag.ToString().ToLower()),
                await SetOutputAsync("is_semver", isSemVer.ToString().ToLower()),
                await SetOutputAsync("tag", tag ?? ""),
                await SetOutputAsync("current_branch_name", currentBranchName ?? ""),
                await SetOutputAsync("is_default_branch", isDefaultBranch.ToString().ToLower()),
                await SetOutputAsync("default_branch_name", defaultBranchName ?? ""),
                await SetOutputAsync("base_branch_name", targetBranchName ?? ""),
                await SetOutputAsync("head_branch_name", sourceBranchName ?? ""),
                await SetOutputAsync("is_pull_request", isPR.ToString().ToLower()),
                await SetOutputAsync("pull_request_id", pullRequestId ?? ""),
                await SetOutputAsync("repo_owner_name", owner.ToLower()),
                await SetOutputAsync("repo_name", _context.Repository.Split("/")[1]),
                await SetOutputAsync("event_actor_name", eventActor),
                await SetOutputAsync("is_event_actor_rerun", isRerun.ToString().ToLower()),
                await SetOutputAsync("is_event_actor_owner", eventActor.Equals(owner, 
                        StringComparison.InvariantCultureIgnoreCase).ToString().ToLower())
            ];

            if (_inputs.ExportVariables.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var kvp in rows.ToDictionary(k => k[0], v => v[1]))
                {
                    await ExportVariableAsync($"BD_{kvp.Key.ToUpper()}", kvp.Value);
                }
            }

            var summary = new Summary();
            summary.AppendHeader("Branch Details GitHub Action", 3);
            summary.AppendParagraph("Below is a list of all the generated outputs for this run.");
            summary.AppendTable(columns, rows);

            await SetStepSummaryAsync(summary.ToString());
            _logger.LogInformation("🎉 Process completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return ExitCode.Failure;
        }

        return ExitCode.Success;
    }
}