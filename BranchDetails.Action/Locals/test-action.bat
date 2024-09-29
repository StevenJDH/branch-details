:: This file is part of Branch Details <https://github.com/StevenJDH/branch-details>.
:: Copyright (C) 2024 Steven Jenkins De Haro.
::
:: Branch Details is free software: you can redistribute it and/or modify
:: it under the terms of the GNU General Public License as published by
:: the Free Software Foundation, either version 3 of the License, or
:: (at your option) any later version.
::
:: Branch Details is distributed in the hope that it will be useful,
:: but WITHOUT ANY WARRANTY; without even the implied warranty of
:: MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
:: GNU General Public License for more details.
::
:: You should have received a copy of the GNU General Public License
:: along with Branch Details.  If not, see <http://www.gnu.org/licenses/>.

@echo off
SETLOCAL

copy "%~1" "%~2"
set prjFilePath="%~2"
set replacementLine=  image: 'docker://ghcr.io/stevenjdh/branch-details:latest'

findstr /i /v "image: 'docker:" %prjFilePath%>%TEMP%\tmp-action.yml
echo %replacementLine%>>%TEMP%\tmp-action.yml
move /y %TEMP%\tmp-action.yml %prjFilePath%