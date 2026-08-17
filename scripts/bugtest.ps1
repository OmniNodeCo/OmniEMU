$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$output = if ($env:OMNIEMU_BUG_REPORT_DIR) { $env:OMNIEMU_BUG_REPORT_DIR } else { Join-Path $root "bug-reports" }

dotnet run `
  --project (Join-Path $root "tools/OmniEMU.BugTester/OmniEMU.BugTester.csproj") `
  --configuration Release `
  -- `
  --source $root `
  --output $output `
  @args

exit $LASTEXITCODE
