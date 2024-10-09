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

using System.Reflection;

namespace BranchDetails.Action.Tests;

[TestFixture]
internal class ProgramTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("INPUT_GITHUB-TOKEN", null);
    }

    [Test, Description("Should load and throw ArgumentException when no access token is provided.")]
    public void Should_LoadAndThrowArgumentException_When_NoAccessTokenIsProvided()
    {
        var program = typeof(Program).GetTypeInfo();
        var mainMethod = program.DeclaredMethods.Single(m => m.Name == "<Main>$");
        const string expectedMessage = "accessToken";

        Assert.That(async () => await (Task<int>)mainMethod.Invoke(null, [Array.Empty<string>()])!, Throws.ArgumentException
            .With.Message.EqualTo(expectedMessage));
    }
}