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

using Microsoft.Extensions.Logging;

namespace BranchDetails.Action.Loggers;

public sealed class GitHubConsoleLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var logEntry = $"{formatter(state, exception)}{(exception != null ? $" {exception.StackTrace}" : "")}";
        
        // ILogger:  Trace, Debug, Info,  ---    Warning, Error, Critical.
        // GitHub:    ---   Debug, Info, Notice, Warning, Error,  ---.
        string ghPrefix = logLevel switch
        {
            LogLevel.Trace => "::debug::Trace: ",
            LogLevel.Debug => "::debug::",
            LogLevel.Information when eventId.Name == "AddMask" => "::add-mask::",
            LogLevel.Information when eventId.Name == "GroupStart" => "::group::",
            LogLevel.Information when eventId.Name == "GroupEnd" => "::endgroup::",
            LogLevel.Information when eventId.Name == "Notice" => "::notice::",
            LogLevel.Information => "",
            LogLevel.Warning => "::warning::",
            LogLevel.Error => "::error::",
            LogLevel.Critical => "::error::Critical: ",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Unknown logging level."),
        };

        Console.WriteLine($"{ghPrefix}{logEntry}");
    }
}