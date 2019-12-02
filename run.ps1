#!/usr/bin/env pwsh

$workingPath = $PWD;

try
{
  Set-Location .\service\MinMQ.Service\
  dotnet run
  
  Set-Location $workingPath

  Set-Location .\service\BenchmarkConsole\
  dotnet run
}
finally
{
  Set-Location $workingPath;
}
