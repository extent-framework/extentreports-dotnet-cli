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

        [Option(ShortName = "p")]
        private TestFramework Parser { get; set; } = Model.TestFramework.NUnit;

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

        private static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

        private void OnExecute()
        {
            _logger = new Logger(LoggingLevel);

            _logger.WriteLine(LoggingLevel.Verbose, "extentreports-cli initializing ...");

            string output = string.IsNullOrWhiteSpace(Output) ? $".\\{DefaultBaseDirectory}" : Output;

            bool foundResultFiles = false;

            if (!string.IsNullOrEmpty(TestRunnerResultsDirectory) && File.GetAttributes(TestRunnerResultsDirectory) == FileAttributes.Directory && Parser.Equals(TestFramework.NUnit))
            {

                string filePattern = "*." + KnownFileExtensions.GetExtension(Parser);
                _logger.WriteLine(LoggingLevel.Normal, $"Getting test runner result files in folder '{TestRunnerResultsDirectory}' matching pattern '{filePattern}' ...");

                List<string> files = Directory.GetFiles(TestRunnerResultsDirectory, filePattern, SearchOption.AllDirectories).ToList();

                foreach (string file in files)
                {
                    _logger.WriteLine(LoggingLevel.Normal, $"Parsing test runner result file '{file}' ...");
                    var extent = new ExtentReports();
                    var dir = Path.Combine(output, Path.GetFileNameWithoutExtension(file));
                    InitializeReporter(extent, dir);
                    new NUnitParser(extent).ParseTestRunnerOutput(file);
                    extent.Flush();
                    _logger.WriteLine(LoggingLevel.Normal, $"Report for '{file}' is complete.");
                    foundResultFiles = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(TestRunnerResultsFile))
            {
                _logger.WriteLine(LoggingLevel.Normal, $"Parsing test runner result file '{TestRunnerResultsFile}' ...");
                var extent = new ExtentReports();
                InitializeReporter(extent, output);
                new NUnitParser(extent).ParseTestRunnerOutput(TestRunnerResultsFile);
                extent.Flush();
                _logger.WriteLine(LoggingLevel.Normal, $"Report for '{TestRunnerResultsFile}' is complete.");
                foundResultFiles = true;
            }

            if (!foundResultFiles)
                _logger.WriteLine(LoggingLevel.Normal, "Nothing to do!");

            _logger.WriteLine(LoggingLevel.Verbose, "extentreports-cli finished.");
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
