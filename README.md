# OmniEMU

OmniEMU is a **real, native Nintendo Switch emulator** for Windows, Linux, and Apple Silicon macOS. It is built from the MIT-licensed Ryujinx emulation architecture and includes a real ARM CPU translator, Horizon OS high-level emulation, Maxwell GPU emulation, Vulkan/OpenGL rendering, audio, controller input, shader caching, and an Avalonia desktop interface.

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
- Windows 10/11 x64, Linux x64, or macOS Apple Silicon (`osx-arm64`)

## Build

Restore and build the native desktop app:

```bash
dotnet restore Ryujinx.sln
dotnet build Ryujinx.sln -c Release
```

Run it from source:

```bash
dotnet run --project src/Ryujinx/Ryujinx.csproj -c Release
```

Publish self-contained builds:

```bash
# Windows x64
dotnet publish src/Ryujinx/Ryujinx.csproj -c Release -r win-x64 --self-contained true -o publish/windows

# Linux x64
dotnet publish src/Ryujinx/Ryujinx.csproj -c Release -r linux-x64 --self-contained true -o publish/linux

# Apple Silicon macOS
dotnet publish src/Ryujinx/Ryujinx.csproj -c Release -r osx-arm64 --self-contained true -o publish/macos-arm64
```

## First run

1. Open the native application.
2. Use **Tools → Install Firmware** with firmware dumped from your own console.
3. Place your own `prod.keys` in the system directory opened through **File → Open App Data Folder**.
4. Add the directory containing your legally dumped games.
5. Configure graphics and input, then launch a title from the library.

## Upstream and license

The emulator implementation is derived from the Ryujinx project and remains available under the MIT License. Original copyright notices are preserved in [`LICENSE.txt`](LICENSE.txt), source headers, and [`distribution/legal/THIRDPARTY.md`](distribution/legal/THIRDPARTY.md).

OmniEMU is not affiliated with or endorsed by Nintendo. Nintendo Switch is a trademark of Nintendo.
