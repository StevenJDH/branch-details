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
using Microsoft.Extensions.Configuration;

namespace BranchDetails.Action.Tests.Extensions;

[TestFixture]
public class ConfigurationBuilderExtensionsTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Switches logic to local mode to activate the logic for loading secrets. 
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }

    [Test, Description("Should load any secrets available for local development.")]
    public void Should_LoadAnySecretsAvailable_ForLocalDevelopment()
    {
        const string expectedKey = "test";
        const string expectedValue = "foobar";
        IConfiguration configRoot = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>(expectedKey, expectedValue)])
            .AddGitHubActionSecrets() // No secrets to load in pipeline, but we ensure it works anyway.
            .Build();

        string? testConfig = configRoot.GetSection(expectedKey).Value;

        Assert.Multiple(() =>
        {
            Assert.That(configRoot.AsEnumerable().Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(testConfig, Is.EqualTo(expectedValue));
        });
    }
}