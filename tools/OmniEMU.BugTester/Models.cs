using System.Text.Json.Serialization;

namespace OmniEMU.BugTester;

public enum Severity
{
    Pass,
    Info,
    Warning,
    Error,
}

public sealed record Finding(
    string Code,
    Severity Severity,
    string Category,
    string Title,
    string Detail,
    string? Remediation = null,
    string? Path = null,
    int? Line = null,
    int Occurrences = 1);

public sealed class DiagnosticReport
{
    public string Product { get; init; } = "OmniEMU";
    public string BugTesterVersion { get; init; } = "0.1.0";
    public DateTimeOffset GeneratedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string Machine { get; init; } = Environment.MachineName;
    public string OperatingSystem { get; init; } = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
    public string Architecture { get; init; } = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
    public string Framework { get; init; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    public string AppDataPath { get; set; } = string.Empty;
    public string? SourcePath { get; set; }
    public List<Finding> Findings { get; } = new();

    [JsonIgnore]
    public int Errors => Findings.Count(f => f.Severity == Severity.Error);

    [JsonIgnore]
    public int Warnings => Findings.Count(f => f.Severity == Severity.Warning);

    [JsonIgnore]
    public int Passed => Findings.Count(f => f.Severity == Severity.Pass);
}
