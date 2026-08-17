using OmniEMU.BugTester;

try
{
    Options options = Options.Parse(args);
    if (options.ShowHelp)
    {
        Options.PrintHelp();
        return 0;
    }

    Console.WriteLine("OmniEMU BugTester 0.1.0");
    Console.WriteLine("Created and maintained by OmniNodeCo");
    Console.WriteLine("Running system, setup, configuration, graphics, crash, log, and source diagnostics…");
    Console.WriteLine();

    DiagnosticReport report = new DiagnosticRunner(options).Run();
    var paths = ReportWriter.Write(report, options.OutputDirectory);

    foreach (Finding finding in report.Findings.Where(f => f.Severity is Severity.Error or Severity.Warning))
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = finding.Severity == Severity.Error ? ConsoleColor.Red : ConsoleColor.Yellow;
        Console.Write($"[{finding.Severity.ToString().ToUpperInvariant()}] {finding.Code}");
        Console.ForegroundColor = previous;
        Console.WriteLine($" · {finding.Title}: {finding.Detail}");
    }

    Console.WriteLine();
    Console.WriteLine($"Result: {report.Errors} errors, {report.Warnings} warnings, {report.Passed} passed checks");
    Console.WriteLine($"JSON: {paths.Json}");
    Console.WriteLine($"Markdown: {paths.Markdown}");

    if (report.Errors > 0 || (options.Strict && report.Warnings > 0))
    {
        return 1;
    }
    return 0;
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Argument error: {exception.Message}");
    Console.Error.WriteLine("Use --help for usage.");
    return 2;
}
catch (Exception exception)
{
    Console.Error.WriteLine("BugTester itself encountered an unexpected error:");
    Console.Error.WriteLine(exception);
    return 3;
}
