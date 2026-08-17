using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OmniEMU.BugTester;

public sealed partial class DiagnosticRunner
{
    private readonly Options _options;
    private readonly DiagnosticReport _report = new();

    public DiagnosticRunner(Options options)
    {
        _options = options;
    }

    public DiagnosticReport Run()
    {
        string appData = Path.GetFullPath(_options.AppDataPath ?? GetDefaultAppDataPath());
        _report.AppDataPath = appData;
        _report.SourcePath = _options.SourcePath is null ? null : Path.GetFullPath(_options.SourcePath);

        CheckPlatform();
        CheckMemory();
        CheckAppData(appData);
        CheckKeysAndFirmware(appData);
        CheckConfiguration(appData);
        CheckGraphicsRuntime();
        CheckCrashArtifacts(appData);
        if (_options.LogCount > 0)
        {
            CheckLogs(appData, _options.LogCount);
        }
        if (_report.SourcePath is not null)
        {
            CheckSource(_report.SourcePath);
        }

        return _report;
    }

    private void Add(string code, Severity severity, string category, string title, string detail,
        string? remediation = null, string? path = null, int? line = null, int occurrences = 1) =>
        _report.Findings.Add(new Finding(code, severity, category, title, detail, remediation, path, line, occurrences));

    private static string GetDefaultAppDataPath()
    {
        string basePath;
        if (OperatingSystem.IsMacOS())
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support");
        }
        else
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
            }
        }
        return Path.Combine(basePath, "OmniEMU");
    }

    private void CheckPlatform()
    {
        Architecture architecture = RuntimeInformation.OSArchitecture;
        bool supported = (OperatingSystem.IsWindows() && architecture == Architecture.X64) ||
                         (OperatingSystem.IsLinux() && architecture == Architecture.X64) ||
                         (OperatingSystem.IsMacOS() && architecture == Architecture.Arm64);
        Add("SYS001", supported ? Severity.Pass : Severity.Error, "System", "Supported platform",
            supported
                ? $"{RuntimeInformation.OSDescription} ({architecture}) is a supported release target."
                : $"No official OmniEMU package is produced for {RuntimeInformation.OSDescription} ({architecture}).",
            supported ? null : "Use Windows x64, Linux x64, or Apple Silicon macOS.");
    }

    private void CheckMemory()
    {
        long bytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        if (bytes <= 0)
        {
            Add("SYS002", Severity.Info, "System", "Memory could not be measured",
                "The runtime did not report total available memory.");
            return;
        }
        double gib = bytes / 1024d / 1024d / 1024d;
        Severity severity = gib < 6 ? Severity.Error : gib < 8 ? Severity.Warning : Severity.Pass;
        Add("SYS002", severity, "System", "Available memory", $"The runtime reports {gib:F1} GiB available.",
            severity == Severity.Pass ? null : "OmniEMU requires at least 8 GiB RAM; 16 GiB or more is recommended.");
    }

    private void CheckAppData(string appData)
    {
        if (!Directory.Exists(appData))
        {
            Add("DATA001", Severity.Warning, "Storage", "Application-data directory is missing",
                $"The directory does not exist: {appData}",
                "Launch OmniEMU once, or pass the correct directory with --app-data.", appData);
            return;
        }

        try
        {
            string probe = Path.Combine(appData, $".bugtester-write-{Guid.NewGuid():N}");
            File.WriteAllText(probe, "OmniEMU diagnostic write test");
            File.Delete(probe);
            Add("DATA002", Severity.Pass, "Storage", "Application-data directory is writable", appData, path: appData);
        }
        catch (Exception exception)
        {
            Add("DATA002", Severity.Error, "Storage", "Application-data directory is not writable",
                exception.Message, "Fix directory ownership/permissions and ensure security software is not blocking OmniEMU.", appData);
        }

        try
        {
            string root = Path.GetPathRoot(appData) ?? appData;
            DriveInfo drive = new(root);
            double freeGib = drive.AvailableFreeSpace / 1024d / 1024d / 1024d;
            Severity severity = freeGib < 2 ? Severity.Error : freeGib < 10 ? Severity.Warning : Severity.Pass;
            Add("DATA003", severity, "Storage", "Free disk space", $"{freeGib:F1} GiB is available on {drive.Name}.",
                severity == Severity.Pass ? null : "Free at least 10 GiB for shader caches, firmware, saves, and updates.");
        }
        catch (Exception exception)
        {
            Add("DATA003", Severity.Info, "Storage", "Disk space could not be measured", exception.Message);
        }
    }

    private void CheckKeysAndFirmware(string appData)
    {
        string keyPath = Path.Combine(appData, "system", "prod.keys");
        if (!File.Exists(keyPath))
        {
            Add("SETUP001", Severity.Warning, "Setup", "prod.keys is missing", keyPath,
                "Export prod.keys from a Nintendo Switch you own and place it in OmniEMU's system directory. Never download or share keys.", keyPath);
        }
        else
        {
            long size = new FileInfo(keyPath).Length;
            Add("SETUP001", size > 0 ? Severity.Pass : Severity.Error, "Setup", "prod.keys detected",
                $"The key file exists and is {size:N0} bytes. Its contents were not read.",
                size > 0 ? null : "Replace the empty file with a valid export from hardware you own.", keyPath);
        }

        string firmwarePath = Path.Combine(appData, "bis", "system", "Contents", "registered");
        int ncaCount = Directory.Exists(firmwarePath)
            ? Directory.EnumerateFiles(firmwarePath, "*.nca", SearchOption.TopDirectoryOnly).Count()
            : 0;
        Add("SETUP002", ncaCount > 0 ? Severity.Pass : Severity.Warning, "Setup", "Firmware content",
            ncaCount > 0 ? $"Detected {ncaCount:N0} registered firmware NCA files." : "No installed firmware content was detected.",
            ncaCount > 0 ? null : "Use Tools → Install Firmware with a firmware dump from a console you own.", firmwarePath);
    }

    private void CheckConfiguration(string appData)
    {
        string configPath = Path.Combine(appData, "Config.json");
        if (!File.Exists(configPath))
        {
            Add("CFG001", Severity.Info, "Configuration", "Configuration has not been created",
                configPath, "Launch OmniEMU once to create the default configuration.", configPath);
            return;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(configPath), new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });
            Add("CFG001", Severity.Pass, "Configuration", "Configuration JSON is valid", configPath, path: configPath);

            foreach (string gameDirectory in FindStringArray(document.RootElement, "game_dirs", "game_directories"))
            {
                if (!Directory.Exists(gameDirectory))
                {
                    Add("CFG002", Severity.Warning, "Configuration", "Configured game directory is missing",
                        gameDirectory, "Remove the stale library path or reconnect the drive containing it.", configPath);
                }
            }
        }
        catch (JsonException exception)
        {
            Add("CFG001", Severity.Error, "Configuration", "Configuration JSON is malformed",
                exception.Message, "Back up Config.json, repair the indicated JSON, or rename it so OmniEMU can regenerate defaults.",
                configPath, exception.LineNumber is null ? null : checked((int)exception.LineNumber.Value + 1));
        }
        catch (Exception exception)
        {
            Add("CFG001", Severity.Error, "Configuration", "Configuration could not be read",
                exception.Message, "Check file permissions and storage health.", configPath);
        }
    }

    private static IEnumerable<string> FindStringArray(JsonElement element, params string[] propertyNames)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            yield break;
        }
        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (propertyNames.Contains(property.Name, StringComparer.OrdinalIgnoreCase) && property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement value in property.Value.EnumerateArray())
                {
                    if (value.ValueKind == JsonValueKind.String && value.GetString() is { Length: > 0 } text)
                    {
                        yield return text;
                    }
                }
            }
        }
    }

    private void CheckGraphicsRuntime()
    {
        string[] names = OperatingSystem.IsWindows()
            ? new[] { "vulkan-1.dll" }
            : OperatingSystem.IsMacOS()
                ? new[] { "libMoltenVK.dylib", "libvulkan.1.dylib" }
                : new[] { "libvulkan.so.1", "libvulkan.so" };

        bool loaded = false;
        string? loadedName = null;
        foreach (string name in names)
        {
            if (NativeLibrary.TryLoad(name, out IntPtr handle))
            {
                loaded = true;
                loadedName = name;
                NativeLibrary.Free(handle);
                break;
            }
        }
        Add("GPU001", loaded ? Severity.Pass : Severity.Warning, "Graphics", "Vulkan runtime",
            loaded ? $"Loaded {loadedName}." : $"Could not load any of: {string.Join(", ", names)}.",
            loaded ? null : "Install current GPU drivers. OmniEMU can fall back to OpenGL, but Vulkan is recommended.");
    }

    private void CheckCrashArtifacts(string appData)
    {
        if (!Directory.Exists(appData))
        {
            return;
        }

        try
        {
            string[] crashFiles = Directory.EnumerateFiles(appData, "*", SearchOption.AllDirectories)
                .Where(path => path.EndsWith(".dmp", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".crash", StringComparison.OrdinalIgnoreCase))
                .Take(100)
                .ToArray();
            if (crashFiles.Length == 0)
            {
                Add("CRASH001", Severity.Pass, "Crashes", "No crash dumps detected", "No .dmp or .crash files were found in application data.");
            }
            else
            {
                foreach (string file in crashFiles)
                {
                    Add("CRASH001", Severity.Error, "Crashes", "Crash artifact detected",
                        $"{Path.GetFileName(file)} ({new FileInfo(file).Length:N0} bytes)",
                        "Attach this crash artifact and the matching log when reporting a reproducible crash.", file);
                }
            }
        }
        catch (Exception exception)
        {
            Add("CRASH001", Severity.Warning, "Crashes", "Crash directory scan was incomplete", exception.Message,
                "Run BugTester with permission to read the OmniEMU app-data directory.", appData);
        }
    }

    private void CheckLogs(string appData, int logCount)
    {
        IEnumerable<string> directories = GetLogDirectories(appData).Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase);
        string[] logs = directories.SelectMany(path => Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly))
            .Where(path => path.EndsWith(".log", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .Take(logCount)
            .ToArray();

        if (logs.Length == 0)
        {
            Add("LOG001", Severity.Info, "Logs", "No logs were found",
                $"Checked: {string.Join(", ", directories)}", "Launch OmniEMU and reproduce the problem before running BugTester again.");
            return;
        }

        Dictionary<string, (string File, int Line, int Count)> errors = new(StringComparer.Ordinal);
        int warnings = 0;
        foreach (string log in logs)
        {
            int lineNumber = 0;
            try
            {
                foreach (string line in File.ReadLines(log))
                {
                    lineNumber++;
                    if (line.Contains(" |W| ", StringComparison.Ordinal))
                    {
                        warnings++;
                    }
                    if (!IsErrorLine(line))
                    {
                        continue;
                    }
                    string normalized = NormalizeLogLine(line);
                    if (errors.TryGetValue(normalized, out var existing))
                    {
                        errors[normalized] = (existing.File, existing.Line, existing.Count + 1);
                    }
                    else
                    {
                        errors[normalized] = (log, lineNumber, 1);
                    }
                }
            }
            catch (Exception exception)
            {
                Add("LOG002", Severity.Warning, "Logs", "A log could not be read", exception.Message, path: log);
            }
        }

        Add("LOG001", errors.Count == 0 ? Severity.Pass : Severity.Error, "Logs", "Runtime error scan",
            errors.Count == 0
                ? $"Scanned {logs.Length} recent logs and found no error-level entries. Warning entries: {warnings:N0}."
                : $"Found {errors.Count:N0} unique error signatures across {logs.Length} logs. Warning entries: {warnings:N0}.",
            errors.Count == 0 ? null : "Review each LOG003 finding, reproduce it on the latest build, and attach the report to a bug issue.");

        foreach ((string message, var location) in errors.OrderBy(pair => pair.Value.File).ThenBy(pair => pair.Value.Line))
        {
            Add("LOG003", Severity.Error, "Logs", "Application error", message,
                "Use the surrounding log lines to identify the failing subsystem; do not publish keys or personal paths.",
                location.File, location.Line, location.Count);
        }
    }

    private static IEnumerable<string> GetLogDirectories(string appData)
    {
        yield return Path.Combine(appData, "Logs");
        yield return Path.Combine(Environment.CurrentDirectory, "Logs");
        if (OperatingSystem.IsMacOS())
        {
            yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Logs", "OmniEMU");
        }
    }

    private static bool IsErrorLine(string line) =>
        line.Contains(" |E| ", StringComparison.Ordinal) ||
        line.Contains("Unhandled exception", StringComparison.OrdinalIgnoreCase) ||
        line.Contains("Fatal error", StringComparison.OrdinalIgnoreCase) ||
        line.Contains("Exception:", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeLogLine(string line)
    {
        string withoutTimestamp = LogPrefixRegex().Replace(line, string.Empty);
        return HexAddressRegex().Replace(withoutTimestamp, "0x…").Trim();
    }

    private void CheckSource(string source)
    {
        string[] required =
        {
            "OmniEMU.sln", "LICENSE.txt", "UPSTREAM.md", "Directory.Build.props",
            "src/OmniEMU/OmniEMU.csproj", "src/OmniEMU/Program.cs",
            ".github/workflows/build.yml", ".github/workflows/release.yml",
        };
        foreach (string relative in required)
        {
            string path = Path.Combine(source, relative.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path))
            {
                Add("SRC001", Severity.Error, "Source", "Required source file is missing", relative,
                    "Restore the file from version control.", path);
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC001" || f.Severity != Severity.Error))
        {
            Add("SRC001", Severity.Pass, "Source", "Required source layout is complete", source, path: source);
        }

        CheckSourceXml(source);
        CheckSourceJson(source);
        CheckProjectReferences(source);
        CheckSourceVersion(source);
        CheckWorkflowPaths(source);
        CheckMergeMarkers(source);
        CheckTechnicalDebt(source);
    }

    private void CheckSourceXml(string source)
    {
        string[] patterns = { "*.csproj", "*.props", "*.targets", "*.axaml", "*.xaml" };
        int count = 0;
        foreach (string path in patterns.SelectMany(pattern => Directory.EnumerateFiles(source, pattern, SearchOption.AllDirectories))
                     .Where(path => !IsGeneratedPath(path)))
        {
            count++;
            try
            {
                XDocument.Load(path, LoadOptions.SetLineInfo);
            }
            catch (Exception exception)
            {
                Add("SRC002", Severity.Error, "Source", "Invalid XML/XAML", exception.Message,
                    "Correct the malformed XML before building.", path);
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC002" || f.Severity != Severity.Error))
        {
            Add("SRC002", Severity.Pass, "Source", "XML/XAML syntax", $"Parsed {count:N0} project and UI files.");
        }
    }

    private void CheckSourceJson(string source)
    {
        int count = 0;
        foreach (string path in Directory.EnumerateFiles(source, "*.json", SearchOption.AllDirectories).Where(path => !IsGeneratedPath(path)))
        {
            count++;
            try
            {
                using JsonDocument _ = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                });
            }
            catch (JsonException exception)
            {
                Add("SRC003", Severity.Error, "Source", "Invalid JSON", exception.Message,
                    "Correct the malformed JSON before building.", path,
                    exception.LineNumber is null ? null : checked((int)exception.LineNumber.Value + 1));
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC003" || f.Severity != Severity.Error))
        {
            Add("SRC003", Severity.Pass, "Source", "JSON syntax", $"Parsed {count:N0} JSON files.");
        }
    }

    private void CheckProjectReferences(string source)
    {
        int count = 0;
        foreach (string project in Directory.EnumerateFiles(Path.Combine(source, "src"), "*.csproj", SearchOption.AllDirectories))
        {
            XDocument document;
            try { document = XDocument.Load(project); }
            catch { continue; }
            foreach (XElement reference in document.Descendants("ProjectReference"))
            {
                count++;
                string? include = reference.Attribute("Include")?.Value;
                if (include is null) continue;
                string target = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project)!, include.Replace('\\', Path.DirectorySeparatorChar)));
                if (!File.Exists(target))
                {
                    Add("SRC004", Severity.Error, "Source", "Broken project reference", include,
                        "Correct the ProjectReference path or restore the missing project.", project);
                }
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC004" || f.Severity != Severity.Error))
        {
            Add("SRC004", Severity.Pass, "Source", "Project references", $"Resolved {count:N0} project references.");
        }
    }

    private void CheckSourceVersion(string source)
    {
        string[] projects =
        {
            "src/OmniEMU/OmniEMU.csproj",
            "src/OmniEMU.Gtk3/OmniEMU.Gtk3.csproj",
            "src/OmniEMU.Headless.SDL2/OmniEMU.Headless.SDL2.csproj",
            "tools/OmniEMU.BugTester/OmniEMU.BugTester.csproj",
        };
        Dictionary<string, string> versions = new();
        foreach (string relative in projects)
        {
            string path = Path.Combine(source, relative.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path)) continue;
            string? version = XDocument.Load(path).Descendants("Version").FirstOrDefault()?.Value;
            versions[relative] = version ?? "(missing)";
        }
        string[] distinct = versions.Values.Distinct().ToArray();
        bool aligned = distinct.Length == 1 && Version.TryParse(distinct[0], out _);
        Add("SRC005", aligned ? Severity.Pass : Severity.Error,
            "Source", "Product version alignment",
            string.Join("; ", versions.Select(pair => $"{pair.Key}={pair.Value}")),
            aligned ? null : "Keep all shipping projects on the same valid semantic version.");
    }

    private void CheckWorkflowPaths(string source)
    {
        string build = Path.Combine(source, ".github", "workflows", "build.yml");
        string release = Path.Combine(source, ".github", "workflows", "release.yml");
        foreach (string path in new[] { build, release }.Where(File.Exists))
        {
            string text = File.ReadAllText(path);
            if (text.Contains("Ryujinx.sln", StringComparison.Ordinal) || text.Contains("src/Ryujinx", StringComparison.Ordinal))
            {
                Add("SRC006", Severity.Error, "Source", "Workflow contains stale project paths", Path.GetFileName(path),
                    "Replace legacy paths with OmniEMU.sln and src/OmniEMU/OmniEMU.csproj.", path);
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC006" || f.Severity != Severity.Error))
        {
            Add("SRC006", Severity.Pass, "Source", "Workflow project paths", "No stale pre-rebrand project paths were found.");
        }
    }

    private void CheckMergeMarkers(string source)
    {
        string[] extensions = { ".cs", ".axaml", ".xaml", ".json", ".yml", ".yaml", ".props", ".csproj", ".md", ".sh" };
        foreach (string path in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)
                     .Where(path => extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase) && !IsGeneratedPath(path)))
        {
            int line = 0;
            foreach (string text in File.ReadLines(path))
            {
                line++;
                if (text.StartsWith("<<<<<<< ", StringComparison.Ordinal) || text == "=======" || text.StartsWith(">>>>>>> ", StringComparison.Ordinal))
                {
                    Add("SRC007", Severity.Error, "Source", "Unresolved merge marker", text,
                        "Resolve the merge conflict and remove all marker lines.", path, line);
                }
            }
        }
        if (_report.Findings.All(f => f.Code != "SRC007" || f.Severity != Severity.Error))
        {
            Add("SRC007", Severity.Pass, "Source", "Merge-conflict scan", "No unresolved conflict markers were found.");
        }
    }

    private void CheckTechnicalDebt(string source)
    {
        int todo = 0;
        int fixme = 0;
        foreach (string path in Directory.EnumerateFiles(Path.Combine(source, "src"), "*.cs", SearchOption.AllDirectories))
        {
            foreach (string line in File.ReadLines(path))
            {
                if (line.Contains("TODO", StringComparison.OrdinalIgnoreCase)) todo++;
                if (line.Contains("FIXME", StringComparison.OrdinalIgnoreCase)) fixme++;
            }
        }
        Add("SRC008", Severity.Info, "Source", "Tracked technical debt",
            $"Found {todo:N0} TODO lines and {fixme:N0} FIXME lines. These are inventory, not automatically confirmed bugs.",
            "Triage high-impact items into tracked GitHub issues with owners and test cases.");
    }

    private static bool IsGeneratedPath(string path) =>
        path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
        path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
        path.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\|[A-Z]\|\s+")]
    private static partial Regex LogPrefixRegex();

    [GeneratedRegex(@"0x[0-9a-fA-F]{6,16}")]
    private static partial Regex HexAddressRegex();
}
