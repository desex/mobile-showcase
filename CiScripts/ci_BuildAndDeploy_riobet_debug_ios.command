#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
BUILDER_DIR="$PROJECT_ROOT/.ci/Sotbifay.Builder"

cd "$BUILDER_DIR"

dotnet nuke --target BuildIOSAndDeploy \
 --iosteam-id секрет \
 --partner-name MockPartnerA \
 --build-type Debug \
 --should-increment true  \
 --iospackage-name секрет \
 --app-store-connect-api-key-id секрет \
 --app-store-connect-api-issuer-id секрет\
 --defines USE_APPMETRICA \
 --remove-defines PARTNER_... \
 --remove-defines PARTNER_... 