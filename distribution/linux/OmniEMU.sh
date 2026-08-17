#!/bin/sh

SCRIPT_DIR=$(dirname "$(realpath "$0")")

if [ -f "$SCRIPT_DIR/OmniEMU.Headless.SDL2" ]; then
    OMNIEMU_BIN="OmniEMU.Headless.SDL2"
fi

if [ -f "$SCRIPT_DIR/OmniEMU" ]; then
    OMNIEMU_BIN="OmniEMU"
fi

if [ -z "$OMNIEMU_BIN" ]; then
    exit 1
fi

COMMAND="env DOTNET_EnableAlternateStackCheck=1"

if command -v gamemoderun > /dev/null 2>&1; then
    COMMAND="$COMMAND gamemoderun"
fi

exec $COMMAND "$SCRIPT_DIR/$OMNIEMU_BIN" "$@"
