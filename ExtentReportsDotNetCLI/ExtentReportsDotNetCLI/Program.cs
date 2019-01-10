using AventStack.ExtentReports.Reporter;

using AventStack.ExtentReports.CLI.Model;
using AventStack.ExtentReports.CLI.Parser;

using McMaster.Extensions.CommandLineUtils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AventStack.ExtentReports.CLI.Extensions;

namespace AventStack.ExtentReports.CLI
{
    class Program
    {
        private const string BaseDirectory = "Reports";

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
            if (!string.IsNullOrEmpty(TestRunnerResultsDirectory) && File.GetAttributes(TestRunnerResultsDirectory) == FileAttributes.Directory && Parser.Equals(TestFramework.NUnit))
            {
                List<string> files = Directory.GetFiles(TestRunnerResultsDirectory, "*." + KnownFileExtensions.GetExtension(Parser), SearchOption.AllDirectories).ToList();

                foreach (string file in files)
                {
                    var extent = new ExtentReports();
                    var dir = Path.Combine(Output, BaseDirectory, Path.GetFileNameWithoutExtension(file));
                    InitializeReporter(extent, dir);
                    new NUnitParser(extent).ParseTestRunnerOutput(file);
                    extent.Flush();
                }
            }

            if (!string.IsNullOrWhiteSpace(TestRunnerResultsFile) && !string.IsNullOrWhiteSpace(Output))
            {
                var extent = new ExtentReports();
                InitializeReporter(extent);
                new NUnitParser(extent).ParseTestRunnerOutput(TestRunnerResultsFile);
                extent.Flush();
            }
        }
        
        private void InitializeReporter(ExtentReports extent, string basePath = null)
        {
            string path = string.IsNullOrEmpty(Output) ? "./" : Output;
            path = Path.Combine(path, BaseDirectory);

            if (!string.IsNullOrEmpty(basePath))
            {
                path = basePath;
            }
            
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
