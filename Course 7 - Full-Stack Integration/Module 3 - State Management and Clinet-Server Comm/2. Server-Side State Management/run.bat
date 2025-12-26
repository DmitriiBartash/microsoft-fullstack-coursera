@echo off
echo Running tests...
dotnet test CurrencyExchangeDashboard.Tests/CurrencyExchangeDashboard.Tests.csproj --verbosity minimal

if %ERRORLEVEL% neq 0 (
    echo Tests failed! Application will not start.
    exit /b 1
)

echo All tests passed! Starting application...
dotnet run --project CurrencyExchangeDashboard/CurrencyExchangeDashboard.csproj
