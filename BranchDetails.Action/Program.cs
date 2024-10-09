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
using BranchDetails.Action;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, appConfig) => appConfig.AddGitHubActionSecrets())
    .ConfigureServices((_, services) => services.AddGitHubActionServices())
    .ConfigureLogging((_, logging) => logging.AddGitHubConsoleLogger())
    .Build();

Console.WriteLine($"""
    Branch Details 1.0.1.24102
    Copyright (C) 2024{(DateTime.Now.Year.Equals(2024) ? "" : $"-{DateTime.Now.Year}")} Steven Jenkins De Haro
    
    """);

var processor = host.Services.GetRequiredService<BranchDetailsProcessor>();
return (int) await processor.StartProcessingBranchAsync();