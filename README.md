# OmniEMU

**Created and maintained by [OmniNodeCo](https://github.com/OmniNodeCo).**

OmniEMU is a **real, native Nintendo Switch emulator** for Windows and Linux. It is built from the MIT-licensed Ryujinx emulation architecture and includes a real ARM CPU translator, Horizon OS high-level emulation, Maxwell GPU emulation, Vulkan/OpenGL rendering, audio, controller input, shader caching, and an Avalonia desktop interface.

This repository no longer contains the earlier browser UI prototype.

## What is emulated

- **CPU:** ARMv8 guest execution through the ARMeilleure JIT and memory manager
- **GPU:** Nintendo Switch Maxwell GPU command processing with Vulkan and OpenGL host backends
- **OS:** Horizon kernel, services, IPC, filesystem, account, time, networking, and applet HLE
- **Audio/video:** Audio backends and NVDEC video decoding
- **Input:** Keyboard, mouse, controllers, Joy-Con layouts, motion, and rumble
- **Content:** XCI, NSP, NCA, NRO, and NSO loading through LibHac
- **Desktop UI:** Native Avalonia application, game library, settings, profiles, mods, DLC, and save management

## Legal requirements

OmniEMU does **not** include games, encryption keys, firmware, title keys, or Nintendo-owned system files. Use only firmware, keys, and game dumps exported from hardware and software you own. Do not request or distribute copyrighted files through this project.

## Requirements

- .NET 8 SDK or newer
- 8 GiB RAM minimum
- Vulkan 1.2+ recommended, or OpenGL 4.5+
- Windows 10/11 x64 or Linux x64

## Build

Restore and build the native desktop app:

```bash
dotnet restore OmniEMU.sln
dotnet build OmniEMU.sln -c Release
```

Run it from source:

```bash
dotnet run --project src/OmniEMU/OmniEMU.csproj -c Release
```

Publish self-contained builds:

```bash
# Windows x64
dotnet publish src/OmniEMU/OmniEMU.csproj -c Release -r win-x64 --self-contained true -o publish/windows

# Linux x64
dotnet publish src/OmniEMU/OmniEMU.csproj -c Release -r linux-x64 --self-contained true -o publish/linux
```

## Game and software downloads

The desktop app includes a download center (**File → Download Games & Software…**, or the download button on the left rail):

- **Browse homebrew** — a curated catalog of free, legally redistributable Switch homebrew games and apps (Super-Haxagon, OpenLara, Homebrew App Store, NX-Shell, Goldleaf, Checkpoint, Moonlight, nx-hbmenu, MissionControl, and more) resolved from each project's official GitHub releases.
- **Direct download** — fetch software from any URL you provide, with a rights-confirmation checkbox, file-type validation (`.nro`, `.nsp`, `.xci`, `.nca`, `.nso`, `.zip`, `.7z`, `.jar`, `.ovl`, …), and sanitized file names.
- **Download queue** — progress, cancel, retry, show-in-folder, and one-click **Launch** for completed homebrew. Zip archives are extracted safely (path-traversal protected), the downloads folder is added to your game library, and the library is rescanned automatically.
- **Integrity** — when GitHub publishes a SHA-256 digest for a release asset, downloads are verified before they are kept.

OmniEMU never includes or links to Nintendo games, firmware, or keys. Catalog entries point only at official homebrew projects; anything else must come from URLs you have the right to use.

## Diagnostics and bug reports

OmniEMU includes a cross-platform BugTester that checks the host system, app-data permissions, setup files, configuration JSON, Vulkan availability, crash artifacts, recent error logs, source integrity, project references, versions, workflows, and unresolved merge markers. It creates complete JSON and Markdown reports without reading or copying key, firmware, game, save, or account contents.

```bash
# Linux
./scripts/bugtest.sh

# Any platform with .NET 8
dotnet run --project tools/OmniEMU.BugTester -- --source . --output ./bug-reports
```

Use `--app-data PATH` to inspect a specific installation, `--logs N` to select how many recent logs to scan, and `--strict` to make warnings return a failing exit code. See [`docs/QUALITY_ROADMAP.md`](docs/QUALITY_ROADMAP.md) for the current bug and feature priorities.

## First run

1. Open the native application.
2. Use **Tools → Install Firmware** with firmware dumped from your own console.
3. Place your own `prod.keys` in the system directory opened through **File → Open App Data Folder**.
4. Add the directory containing your legally dumped games.
5. Configure graphics and input, then launch a title from the library.

## Upstream and license

The emulator implementation is derived from the Ryujinx project and remains available under the MIT License. Original copyright notices are preserved in [`LICENSE.txt`](LICENSE.txt), source headers, and [`distribution/legal/THIRDPARTY.md`](distribution/legal/THIRDPARTY.md).

OmniEMU is not affiliated with or endorsed by Nintendo. Nintendo Switch is a trademark of Nintendo.
