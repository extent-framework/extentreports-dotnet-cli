using AventStack.ExtentReports.CLI.Extensions;
using AventStack.ExtentReports.CLI.Model;
using AventStack.ExtentReports.CLI.Parser;
using AventStack.ExtentReports.Reporter;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AventStack.ExtentReports.CLI
{
    class Program
    {
        private const string DefaultBaseDirectory = "Reports";

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

        static void Main(string[] args)
        {
            Console.WriteLine("extentreports-cli initializing..");
            CommandLineApplication.Execute<Program>(args);
        }

        private void OnExecute()
        {

            string output = string.IsNullOrWhiteSpace(Output) ? $".\\{DefaultBaseDirectory}" : Output;

            if (!string.IsNullOrEmpty(TestRunnerResultsDirectory) && File.GetAttributes(TestRunnerResultsDirectory) == FileAttributes.Directory && Parser.Equals(TestFramework.NUnit))
            {
                List<string> files = Directory.GetFiles(TestRunnerResultsDirectory, "*." + KnownFileExtensions.GetExtension(Parser), SearchOption.AllDirectories).ToList();

                foreach (string file in files)
                {
                    var extent = new ExtentReports();
                    var dir = Path.Combine(output, Path.GetFileNameWithoutExtension(file));
                    InitializeReporter(extent, dir);
                    new NUnitParser(extent).ParseTestRunnerOutput(file);
                    extent.Flush();
                }
            }

            if (!string.IsNullOrWhiteSpace(TestRunnerResultsFile))
            {
                var extent = new ExtentReports();
                InitializeReporter(extent, output);
                new NUnitParser(extent).ParseTestRunnerOutput(TestRunnerResultsFile);
                extent.Flush();
            }
        }

        private void InitializeReporter(ExtentReports extent, string path)
        {

            if (Reporters.Contains("html"))
            {
                var output = path.EndsWith("\\") || path.EndsWith("/") ? path : path + "\\";
                extent.AttachReporter(new ExtentHtmlReporter(output));
            }

            if (Reporters.Contains("v3html"))
            {
                var output = Path.Combine(path, "index.html");
                extent.AttachReporter(new ExtentV3HtmlReporter(output));
            }
        }
    }
}
