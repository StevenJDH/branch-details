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


// Ignore Spelling: Sem Ver Validator Versioning

using System.Text.RegularExpressions;

namespace BranchDetails.Action.Validators;

public static partial class SemVerValidator
{
    [GeneratedRegex(@"^v?[0-9]+\.[0-9]+\.[0-9]+$", RegexOptions.Compiled, matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchIfValidSemVer();

    public static bool IsValid(string version) => MatchIfValidSemVer().IsMatch(version);
}