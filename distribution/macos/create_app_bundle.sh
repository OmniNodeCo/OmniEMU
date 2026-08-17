#!/bin/bash

set -e

PUBLISH_DIRECTORY=$1
OUTPUT_DIRECTORY=$2
ENTITLEMENTS_FILE_PATH=$3

APP_BUNDLE_DIRECTORY="$OUTPUT_DIRECTORY/OmniEMU.app"

rm -rf "$APP_BUNDLE_DIRECTORY"
mkdir -p "$APP_BUNDLE_DIRECTORY/Contents"
mkdir "$APP_BUNDLE_DIRECTORY/Contents/Frameworks"
mkdir "$APP_BUNDLE_DIRECTORY/Contents/MacOS"
mkdir "$APP_BUNDLE_DIRECTORY/Contents/Resources"

# Copy executable and nsure executable can be executed
cp "$PUBLISH_DIRECTORY/OmniEMU" "$APP_BUNDLE_DIRECTORY/Contents/MacOS/OmniEMU"
chmod u+x "$APP_BUNDLE_DIRECTORY/Contents/MacOS/OmniEMU"

# Then all libraries
cp "$PUBLISH_DIRECTORY"/*.dylib "$APP_BUNDLE_DIRECTORY/Contents/Frameworks"

# Then resources and mandatory legal/provenance notices.
cp Info.plist "$APP_BUNDLE_DIRECTORY/Contents"
cp OmniEMU.icns "$APP_BUNDLE_DIRECTORY/Contents/Resources/OmniEMU.icns"
cp updater.sh "$APP_BUNDLE_DIRECTORY/Contents/Resources/updater.sh"
for notice in THIRDPARTY.md LICENSE.txt UPSTREAM.md; do
    if [ -f "$PUBLISH_DIRECTORY/$notice" ]; then
        cp "$PUBLISH_DIRECTORY/$notice" "$APP_BUNDLE_DIRECTORY/Contents/Resources/$notice"
    fi
done

echo -n "APPL????" > "$APP_BUNDLE_DIRECTORY/Contents/PkgInfo"

# Fixup libraries and executable
python3 bundle_fix_up.py "$APP_BUNDLE_DIRECTORY" MacOS/OmniEMU

# Now sign it
if ! [ -x "$(command -v codesign)" ];
then
    if ! [ -x "$(command -v rcodesign)" ];
    then
        echo "Cannot find rcodesign on your system, please install rcodesign."
        exit 1
    fi

    # cargo install apple-codesign
    echo "Usign rcodesign for ad-hoc signing"
    rcodesign sign --entitlements-xml-path "$ENTITLEMENTS_FILE_PATH" "$APP_BUNDLE_DIRECTORY"
else
    echo "Usign codesign for ad-hoc signing"
    codesign --entitlements "$ENTITLEMENTS_FILE_PATH" -f --deep -s - "$APP_BUNDLE_DIRECTORY"
fi