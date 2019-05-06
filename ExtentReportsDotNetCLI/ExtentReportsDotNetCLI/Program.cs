using AventStack.ExtentReports.CLI.Extensions;
using AventStack.ExtentReports.CLI.Model;
using AventStack.ExtentReports.CLI.Parser;
using AventStack.ExtentReports.Reporter;

using McMaster.Extensions.CommandLineUtils;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AventStack.ExtentReports.CLI
{
    internal class Program
    {
        private const string DefaultBaseDirectory = "Reports";
        private Logger _logger;
        private ExtentReports _extent = new ExtentReports();
        private int _filesProcessed = 0;

        [Option(ShortName = "p")]
        private TestFramework Parser { get; set; } = TestFramework.NUnit;

        [Option(ShortName = "i")]
        private string TestRunnerResultsFile { get; set; }

        [Option(ShortName = "d")]
        private string TestRunnerResultsDirectory { get; set; }

        [Option(ShortName = "o")]
        private string Output { get; set; }

        [Option(ShortName = "r")]
        private List<string> Reporters { get; set; } = new List<string>() { "html" };

        [Option(ShortName = "l")]
        private LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Normal;

        [Option("--merge")]
        private bool Merge { get; set; } = false;


        private static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

        private void OnExecute()
        {
            _logger = new Logger(LoggingLevel);
            _logger.WriteLine(LoggingLevel.Verbose, "extentreports-cli initializing ...");

            string output = string.IsNullOrWhiteSpace(Output) ? $".\\{DefaultBaseDirectory}" : Output;

            if (!string.IsNullOrEmpty(TestRunnerResultsDirectory) &&
                File.GetAttributes(TestRunnerResultsDirectory).HasFlag(FileAttributes.Directory) &&
                Parser.Equals(TestFramework.NUnit))
            {
                string filePattern = "*." + KnownFileExtensions.GetExtension(Parser);
                _logger.WriteLine(LoggingLevel.Normal, $"Getting test runner result files in folder '{TestRunnerResultsDirectory}' matching pattern '{filePattern}' ...");

                List<string> files = Directory.GetFiles(TestRunnerResultsDirectory, filePattern, SearchOption.AllDirectories).ToList();

                if (Merge)
                {
                    files.ForEach(x => ProcessSingle(x, output, true));
                }
                else
                {
                    files.ForEach(x =>
                    {
                        var dir = Path.Combine(output, Path.GetFileNameWithoutExtension(x));
                        ProcessSingle(x, dir, false);
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(TestRunnerResultsFile))
            {
                ProcessSingle(TestRunnerResultsFile, output, true);
            }

            if (_filesProcessed == 0)
            {
                _logger.WriteLine(LoggingLevel.Normal, "Nothing to do!");
            }

            _logger.WriteLine(LoggingLevel.Verbose, "extentreports-cli finished.");
        }

        private void ProcessSingle(string testResultsFilePath, string output, bool merge = false)
        {
            _logger.WriteLine(LoggingLevel.Normal, $"Parsing test runner result file '{testResultsFilePath}' ...");

            // if a single report is required for each test results file (ie, no merges): 
            // must instantiate every time to clear pre-existing data
            if (!merge)
            {
                _extent = new ExtentReports();
            }

            if ((merge && !_extent.StartedReporterList.Any()) || !merge)
            {
                InitializeReporter(_extent, output);
            }

            new NUnitParser(_extent).ParseTestRunnerOutput(testResultsFilePath);
            _extent.Flush();
            _filesProcessed++;

            _logger.WriteLine(LoggingLevel.Normal, $"Report for '{testResultsFilePath}' is complete.");
        }

        private void InitializeReporter(ExtentReports extent, string path)
        {
            if (Reporters.Contains("html"))
            {
                var output = path.EndsWith("\\") || path.EndsWith("/") ? path : path + "\\";
                extent.AttachReporter(new ExtentHtmlReporter(output));
                _logger.WriteLine(LoggingLevel.Normal, $"The html report will be output to the folder '{output}'.");
            }

            if (Reporters.Contains("v3html"))
            {
                var output = Path.Combine(path, "index.html");
                extent.AttachReporter(new ExtentV3HtmlReporter(output));
                _logger.WriteLine(LoggingLevel.Normal, $"The v3html report will be output to '{output}'.");
            }
        }
    }
}
