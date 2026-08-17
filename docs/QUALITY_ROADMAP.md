# OmniEMU quality and feature roadmap

This is the initial engineering triage for OmniEMU 0.1.x. Items are not claims that every listed subsystem is currently broken; they identify the highest-value work required to turn a functioning emulator core into a dependable maintained product.

## Release blockers and high-priority bugs

1. **Signed update verification** — the updater downloads releases over HTTPS, but it should verify the published SHA-256 sidecar and eventually a signature before replacing application files.
2. **Updater integration tests** — exercise upgrade, rollback, interrupted download, read-only installation, low disk space, archive traversal, malformed release metadata, and GitHub rate limiting on every supported OS.
3. **Crash recovery and safe mode** — detect repeated startup crashes and offer launch with default graphics, disabled mods, and a backed-up configuration.
4. **GPU device-loss recovery** — handle Vulkan device loss and driver resets with a useful report rather than an unexplained exit.
5. **Configuration migrations** — version configuration schemas and test migrations, including detection of older pre-OmniEMU data directories without moving user data unexpectedly.
6. **macOS distribution signing** — release builds are ad-hoc signed. Production distribution should use Developer ID signing and notarization.
7. **Dependency and upstream refresh** — the imported emulator core must be audited and regularly rebased against maintained, legally compatible upstream fixes.
8. **Generated nullable warnings** — source-generator output currently emits nullable-context warnings. Add explicit nullable directives in generators and keep CI warning output actionable.

## Compatibility work

- Maintain a legal homebrew-based smoke-test suite for CPU, GPU, audio, input, filesystem, applets, and Horizon services.
- Add repeatable game-compatibility reports without distributing games, firmware, keys, or proprietary test content.
- Test Vulkan and OpenGL on AMD, Intel, NVIDIA, Apple Silicon/MoltenVK, and Mesa.
- Build controller hot-plug, motion, rumble, dead-zone, and multi-controller regression tests.
- Track shader-cache correctness and stutter across driver updates.
- Expand ARM64 JIT correctness and performance testing on Apple Silicon.
- Add save-data backup/restore and corruption detection before writes.

## Product features

- In-app diagnostics viewer backed by OmniEMU BugTester reports.
- One-click sanitized bug-report bundle containing configuration metadata and logs, never keys or game data.
- Per-game compatibility notes, graphics profiles, and known-workaround suggestions.
- Update channels (stable, preview, nightly) with downgrade protection.
- Update rollback when a newly installed build cannot start.
- Accessible keyboard navigation, screen-reader labels, scalable text, reduced motion, and contrast testing for the custom UI.
- Move newly added hard-coded interface text into all localization catalogs.
- Performance overlay and trace export for CPU, GPU, shader, audio, and frame-pacing diagnosis.
- First-run setup validation for firmware, keys, game directories, graphics API, and controller input.

## Running BugTester

From Linux or macOS:

```bash
./scripts/bugtest.sh
```

From Windows PowerShell:

```powershell
./scripts/bugtest.ps1
```

Scan a user installation without a source checkout:

```bash
dotnet run --project tools/OmniEMU.BugTester -- \
  --app-data /path/to/OmniEMU \
  --output ./bug-reports
```

Use `--strict` in CI to fail on warnings as well as errors. Reports are written as JSON and Markdown. BugTester checks metadata and logs only; it never reads or copies the contents of keys, firmware, games, saves, or account files.
