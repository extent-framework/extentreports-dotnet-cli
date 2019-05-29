using AventStack.ExtentReports.MarkupUtils;

using AventStack.ExtentReports.CLI.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AventStack.ExtentReports.CLI.Parser
{
    internal class JUnitParser : IParser
    {
        private ExtentReports _extent;

        public JUnitParser(ExtentReports extent)
        {
            _extent = extent;
        }

        public void ParseTestRunnerOutput(string resultsFile)
        {
            var doc = XDocument.Load(resultsFile);

            if (doc.Root == null)
            {
                throw new NullReferenceException("Root element not found for " + resultsFile);
            }

            AddSystemInformation(doc);

            var suites = doc
                .Descendants("testsuite");

            foreach (var ts in suites.ToList())
            {
                var test = _extent.CreateTest(ts.Attribute("name").Value);

                // Test Cases
                foreach (var tc in ts.Descendants("testcase").ToList())
                {
                    var node = CreateNode(tc, test);

                    AssignStatusAndMessage(tc, node);
                }
            }
        }

        private static ExtentTest CreateNode(XElement tc, ExtentTest test)
        {
            var name = tc.Attribute("name").Value;
            var description = string.Empty;
            var node = test.CreateNode(name, description);
            return node;
        }

        private static void AssignStatusAndMessage(XElement tc, ExtentTest test)
        {
            var error = tc.Elements().FirstOrDefault();

            if (error != null)
            {
                var statusMessage = "Type: " + error.Attribute("type") + Environment.NewLine;
                statusMessage += "Message: " + error.Attribute("message");

                test.Fail(statusMessage);
            }
            else
            {
                test.Pass(string.Empty);
            }
        }

        private void AddSystemInformation(XDocument doc)
        {
            var suite = doc.Descendants("testsuite").FirstOrDefault();
            if (suite == null)
                return;

            _extent.AddSystemInfo("Machine Name", suite.Attribute("hostname").Value);
        }
    }
}
