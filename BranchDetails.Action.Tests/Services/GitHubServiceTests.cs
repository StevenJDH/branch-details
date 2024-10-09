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
using GitHub.Models;
using GitHub.Octokit.Client;
using GitHub.Octokit.Client.Authentication;
using Microsoft.Kiota.Abstractions.Authentication;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Kiota.Abstractions;

namespace BranchDetails.Action.Tests.Services;

[TestFixture]
public class GitHubServiceTests
{
    private readonly JsonSerializerOptions _githubSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

[OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY_OWNER", "StevenJDH");
        Environment.SetEnvironmentVariable("GITHUB_REPOSITORY", "StevenJDH/branch-details");
    }

    [Test, Description("Should return default branch name when making a request.")]
    public async Task Should_ReturnDefaultBranchName_When_MakingARequest()
    {
        // Arrange
        var ctx = new Context();
        var tokenProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider("123456"));
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        using var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        IRequestAdapter requestAdapter = RequestAdapter.Create(tokenProvider, httpClient);
        var service = new GitHubService(new GitHubClient(requestAdapter), ctx);
        string jsonResponse = JsonSerializer.Serialize(new FullRepository
        {
            DefaultBranch = "foobar"
        }, _githubSerializerOptions);
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
        const string expectedBranchName = "foobar";

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        string? branchName = await service.GetDefaultBranchAsync();

        // Assert
        Assert.That(branchName, Is.EqualTo(expectedBranchName));
    }
}