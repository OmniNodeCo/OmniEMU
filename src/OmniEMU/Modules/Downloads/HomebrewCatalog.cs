using System.Collections.Generic;

namespace OmniEMU.Modules
{
    /// <summary>
    /// Curated catalog of free, legally redistributable Nintendo Switch homebrew.
    /// Every entry links to an official project repository and downloads only the
    /// project's own published release assets. Nintendo games, firmware, keys and
    /// other copyrighted material are never included or linked from here.
    /// </summary>
    internal static class HomebrewCatalog
    {
        public static readonly IReadOnlyList<CatalogEntry> Entries = new List<CatalogEntry>
        {
            // ---- Games ----
            new()
            {
                Title = "Super-Haxagon",
                Author = "RedTopper",
                Category = "Games",
                License = "GPL-3.0",
                Description = "Fast-paced Super Hexagon-inspired arcade game with community levels and a built-in editor. Fully free.",
                PageUrl = "https://github.com/RedTopper/Super-Haxagon",
                Repo = "RedTopper/Super-Haxagon",
                AssetName = "SuperHaxagon-Switch-arm64.zip",
            },
            new()
            {
                Title = "OpenLara",
                Author = "XProger",
                Category = "Games",
                License = "BSD-2-Clause",
                Description = "Open-source Tomb Raider engine port. Free engine — bring game data from a copy you legally own.",
                PageUrl = "https://github.com/XProger/OpenLara",
                Repo = "XProger/OpenLara",
                AssetName = "OpenLara_switch.zip",
            },

            // ---- Apps ----
            new()
            {
                Title = "Homebrew App Store",
                Author = "For The Users",
                Category = "Apps",
                License = "GPL-3.0",
                Description = "The classic homebrew store for Switch. Browse and install hundreds more free apps and games inside OmniEMU.",
                PageUrl = "https://github.com/fortheusers/hb-appstore",
                Repo = "fortheusers/hb-appstore",
                AssetName = "appstore.nro",
            },
            new()
            {
                Title = "NX-Shell",
                Author = "joel16",
                Category = "Apps",
                License = "GPL-2.0",
                Description = "Multi-purpose file manager for the Switch: browse, copy, move, delete and launch files.",
                PageUrl = "https://github.com/joel16/NX-Shell",
                Repo = "joel16/NX-Shell",
                AssetName = "NX-Shell.nro",
            },
            new()
            {
                Title = "Goldleaf",
                Author = "XorTroll",
                Category = "Apps",
                License = "GPL-3.0",
                Description = "Multipurpose homebrew tool: browse content you own, manage saves and transfer files from a PC.",
                PageUrl = "https://github.com/XorTroll/Goldleaf",
                Repo = "XorTroll/Goldleaf",
                AssetName = "Goldleaf.nro",
            },
            new()
            {
                Title = "Checkpoint",
                Author = "BernardoGiordano",
                Category = "Apps",
                License = "GPL-3.0",
                Description = "Fast and simple save backup and restore for your games.",
                PageUrl = "https://github.com/BernardoGiordano/Checkpoint",
                Repo = "BernardoGiordano/Checkpoint",
                AssetName = "Checkpoint.nro",
            },
            new()
            {
                Title = "Moonlight",
                Author = "XITRIX",
                Category = "Apps",
                License = "GPL-3.0",
                Description = "Game streaming client — play games from your own PC on the big screen through OmniEMU or a console.",
                PageUrl = "https://github.com/XITRIX/Moonlight-Switch",
                Repo = "XITRIX/Moonlight-Switch",
                AssetName = "Moonlight-Switch.nro",
            },
            new()
            {
                Title = "Homebrew Details",
                Author = "Chrscool8",
                Category = "Apps",
                License = "GPL-3.0",
                Description = "A homebrew app manager and detail browser — an alternative launcher for your installed titles.",
                PageUrl = "https://github.com/Chrscool8/Homebrew-Details",
                Repo = "Chrscool8/Homebrew-Details",
                AssetName = "homebrew_details.nro",
            },

            // ---- Tools ----
            new()
            {
                Title = "nx-hbmenu",
                Author = "switchbrew",
                Category = "Tools",
                License = "ISC",
                Description = "The official Nintendo Switch Homebrew Menu — the standard launcher for .nro homebrew.",
                PageUrl = "https://github.com/switchbrew/nx-hbmenu",
                Repo = "switchbrew/nx-hbmenu",
                PreferredAssetExtension = ".zip",
            },
            new()
            {
                Title = "MissionControl",
                Author = "ndeadly",
                Category = "Tools",
                License = "GPL-2.0",
                Description = "Use Bluetooth controllers from other consoles natively. Requires a homebrew-capable SD card setup.",
                PageUrl = "https://github.com/ndeadly/MissionControl",
                Repo = "ndeadly/MissionControl",
                PreferredAssetExtension = ".zip",
            },
            new()
            {
                Title = "sys-con",
                Author = "cathery",
                Category = "Tools",
                License = "GPL-3.0",
                Description = "Sysmodule adding third-party wired controller support. Requires a homebrew-capable SD card setup.",
                PageUrl = "https://github.com/cathery/sys-con",
                Repo = "cathery/sys-con",
                PreferredAssetExtension = ".zip",
            },
            new()
            {
                Title = "Ultrahand-Overlay",
                Author = "ppkantorski",
                Category = "Tools",
                License = "GPL-2.0",
                Description = "Craftable system overlay with handy shortcuts, tweaks and a powerful package system.",
                PageUrl = "https://github.com/ppkantorski/Ultrahand-Overlay",
                Repo = "ppkantorski/Ultrahand-Overlay",
                AssetName = "ovlmenu.ovl",
            },
            new()
            {
                Title = "uLaunch",
                Author = "XorTroll",
                Category = "Tools",
                License = "GPL-2.0",
                Description = "Themeable replacement HOME menu for your Switch.",
                PageUrl = "https://github.com/XorTroll/uLaunch",
                Repo = "XorTroll/uLaunch",
                AssetName = "uLaunch.zip",
            },
        };
    }
}
