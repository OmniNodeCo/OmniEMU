#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
output="${OMNIEMU_BUG_REPORT_DIR:-$root/bug-reports}"

exec dotnet run \
  --project "$root/tools/OmniEMU.BugTester/OmniEMU.BugTester.csproj" \
  --configuration Release \
  -- \
  --source "$root" \
  --output "$output" \
  "$@"
