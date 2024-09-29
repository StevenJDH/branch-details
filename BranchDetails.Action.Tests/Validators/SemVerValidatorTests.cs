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

using BranchDetails.Action.Validators;

namespace BranchDetails.Action.Tests.Validators;

[TestFixture]
public class SemVerValidatorTests
{
    [TestCase("v1.0.0")]
    [TestCase("1.0.0")]
    [TestCase("v0.0.0")]
    [TestCase("0.0.0")]
    [TestCase("v11.12.13")]
    [TestCase("11.12.13")]
    [TestCase("123.123.123456")]
    [Description("Should pass validation for values using semantic versioning.")]
    public void Should_PassValidation_ForValuesUsingSemanticVersioning(string version)
    {
        bool result = SemVerValidator.IsValid(version);

        Assert.That(result, Is.True);
    }

    [TestCase("v-1.0.0")]
    [TestCase("A.B.C")]
    [TestCase("1.0")]
    [TestCase("v1")]
    [TestCase("v11.12")]
    [TestCase("11,12,13")]
    [TestCase("V123.123.123456")]
    [TestCase("v1.0.0-beta")]
    [Description("Should fail validation for values not using semantic versioning.")]
    public void Should_FailValidation_ForValuesNotUsingSemanticVersioning(string version)
    {
        bool result = SemVerValidator.IsValid(version);

        Assert.That(result, Is.False);
    }
}