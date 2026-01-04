@echo off
chcp 65001 >nul
setlocal

set "ROOT=%~dp0"
set "API_PROJECT=%ROOT%src\InventoryHub.API"
set "CLIENT_PROJECT=%ROOT%src\InventoryHub.Client"

echo ════════════════════════════════════════════════════════════════
echo   InventoryHub Pro - Launcher
echo ════════════════════════════════════════════════════════════════
echo.

if "%1"=="" goto :both
if /i "%1"=="api" goto :api
if /i "%1"=="client" goto :client
if /i "%1"=="restore" goto :restore
if /i "%1"=="build" goto :build
if /i "%1"=="help" goto :help
goto :help

:both
echo [1/2] Starting API on http://localhost:5000 ...
start "InventoryHub API" cmd /k "cd /d "%API_PROJECT%" && dotnet run --launch-profile http"

echo [2/2] Starting Client on http://localhost:5001 ...
timeout /t 3 /nobreak >nul
start "InventoryHub Client" cmd /k "cd /d "%CLIENT_PROJECT%" && dotnet run --launch-profile http"

echo.
echo ════════════════════════════════════════════════════════════════
echo   Both services started!
echo   API:    http://localhost:5000
echo   Client: http://localhost:5001
echo ════════════════════════════════════════════════════════════════
echo.
echo Opening browser in 5 seconds...
timeout /t 5 /nobreak >nul
start http://localhost:5001
goto :end

:api
echo Starting API only on http://localhost:5000 ...
cd /d "%API_PROJECT%"
dotnet run --launch-profile http
goto :end

:client
echo Starting Client only on http://localhost:5001 ...
cd /d "%CLIENT_PROJECT%"
dotnet run --launch-profile http
goto :end

:restore
echo Restoring packages...
cd /d "%ROOT%"
dotnet restore InventoryHub.sln
goto :end

:build
echo Building solution...
cd /d "%ROOT%"
dotnet build InventoryHub.sln
goto :end

:help
echo.
echo Usage: run.bat [command]
echo.
echo Commands:
echo   (none)    Start both API and Client (default)
echo   api       Start API only
echo   client    Start Client only
echo   restore   Restore NuGet packages
echo   build     Build solution
echo   help      Show this help
echo.
goto :end

:end
endlocal
