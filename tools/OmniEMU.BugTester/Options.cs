namespace OmniEMU.BugTester;

public sealed class Options
{
    public string? AppDataPath { get; private set; }
    public string? SourcePath { get; private set; }
    public string OutputDirectory { get; private set; } = Path.Combine(Environment.CurrentDirectory, "bug-reports");
    public int LogCount { get; private set; } = 10;
    public bool Strict { get; private set; }
    public bool ShowHelp { get; private set; }

    public static Options Parse(string[] args)
    {
        Options options = new();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--app-data":
                    options.AppDataPath = RequireValue(args, ref i, "--app-data");
                    break;
                case "--source":
                    options.SourcePath = RequireValue(args, ref i, "--source");
                    break;
                case "--output":
                    options.OutputDirectory = RequireValue(args, ref i, "--output");
                    break;
                case "--logs":
                    if (!int.TryParse(RequireValue(args, ref i, "--logs"), out int count) || count < 0)
                    {
                        throw new ArgumentException("--logs must be zero or a positive integer.");
                    }
                    options.LogCount = count;
                    break;
                case "--strict":
                    options.Strict = true;
                    break;
                case "--help":
                case "-h":
                    options.ShowHelp = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }
        return options;
    }

    private static string RequireValue(string[] args, ref int index, string option)
    {
        if (++index >= args.Length || args[index].StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"{option} requires a value.");
        }
        return args[index];
    }

    public static void PrintHelp()
    {
        Console.WriteLine("""
OmniEMU BugTester 0.1.0

Usage:
  dotnet run --project tools/OmniEMU.BugTester -- [options]

Options:
  --app-data PATH   Override the OmniEMU application-data directory.
  --source PATH     Also inspect an OmniEMU source checkout.
  --output PATH     Report directory (default: ./bug-reports).
  --logs COUNT      Number of newest logs to inspect (default: 10; 0 disables).
  --strict          Return failure when warnings are found, not only errors.
  --help, -h        Show this help.

Outputs:
  omniemu-bug-report.json   Machine-readable full report.
  omniemu-bug-report.md     Human-readable report with remediation steps.

The report never reads or copies the contents of encryption keys, firmware, games,
saves, or account data. It only checks presence, metadata, configuration, and logs.
""");
    }
}
