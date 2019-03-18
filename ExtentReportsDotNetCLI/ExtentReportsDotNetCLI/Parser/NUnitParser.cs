using AventStack.ExtentReports.MarkupUtils;

using AventStack.ExtentReports.CLI.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AventStack.ExtentReports.CLI.Parser
{
    internal class NUnitParser : IParser
    {
        private ExtentReports _extent;

        public NUnitParser(ExtentReports extent)
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
                .Descendants("test-suite")
                .Where(x => x.Attribute("type").Value.Equals("TestFixture", StringComparison.CurrentCultureIgnoreCase));

            foreach (var ts in suites.ToList())
            {
                var test = _extent.CreateTest(ts.Attribute("name").Value);

                // any error messages and/or stack-trace
                var failure = ts.Element("failure");
                if (failure != null)
                {
                    var message = failure.Element("message");
                    if (message != null)
                    {
                        test.Fail(message.Value);
                    }

                    var stacktrace = failure.Element("stack-trace");
                    if (stacktrace != null && !string.IsNullOrWhiteSpace(stacktrace.Value))
                    {
                        test.Fail(MarkupHelper.CreateCodeBlock(stacktrace.Value));
                    }
                }

                var output = ts.Element("output")?.Value;
                if (!string.IsNullOrWhiteSpace(output))
                {
                    test.Info(output);
                }

                // get test suite level categories
                var suiteCategories = ParseTags(ts, false);

                // Test Cases
                foreach (var tc in ts.Descendants("test-case").ToList())
                {
                    var node = CreateNode(tc, test);

                    AssignStatusAndMessage(tc, node);
                    AssignTags(tc, node);
                }
            }
        }

        private static ExtentTest CreateNode(XElement tc, ExtentTest test)
        {
            var name = tc.Attribute("name").Value;
            var descriptions =
                tc.Descendants("property")
                .Where(c => c.Attribute("name").Value.Equals("Description", StringComparison.CurrentCultureIgnoreCase));
            var description = descriptions.Any() ? descriptions.ToArray()[0].Attribute("value").Value : string.Empty;
            var node = test.CreateNode(name, description);
            return node;
        }

        private static void AssignStatusAndMessage(XElement tc, ExtentTest test)
        {
            var status = StatusExtensions.ToStatus(tc.Attribute("result").Value);
            
            // error and other status messages
            var statusMessage = tc.Element("failure") != null ? tc.Element("failure").Element("message").Value.Trim() : string.Empty;
            statusMessage += tc.Element("failure") != null && tc.Element("failure").Element("stack-trace") != null ? tc.Element("failure").Element("stack-trace").Value.Trim() : string.Empty;
            statusMessage += tc.Element("reason") != null && tc.Element("reason").Element("message") != null ? tc.Element("reason").Element("message").Value.Trim() : string.Empty;
            statusMessage += tc.Element("output") != null ? tc.Element("output").Value.Trim() : string.Empty;
            statusMessage = (status == Status.Fail || status == Status.Error) ? MarkupHelper.CreateCodeBlock(statusMessage).GetMarkup() : statusMessage;
            statusMessage = string.IsNullOrEmpty(statusMessage) ? status.ToString() : statusMessage;
            test.Log(status, statusMessage);
        }

        private static void AssignTags(XElement tc, ExtentTest test)
        {
            // get test case level categories
            var categories = ParseTags(tc, true);

            // if this is a parameterized test, get the categories from the parent test-suite
            var parameterizedTestElement = tc
                .Ancestors("test-suite").ToList()
                .Where(x => x.Attribute("type").Value.Equals("ParameterizedTest", StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();

            if (null != parameterizedTestElement)
            {
                var paramCategories = ParseTags(parameterizedTestElement, false);
                categories.UnionWith(paramCategories);
            }

            categories.ToList().ForEach(x => test.AssignCategory(x));
        }

        private static HashSet<string> ParseTags(XElement elem, bool allDescendents)
        {
            var parser = allDescendents
                ? new Func<XElement, string, IEnumerable<XElement>>((e, s) => e.Descendants(s))
                : new Func<XElement, string, IEnumerable<XElement>>((e, s) => e.Elements(s));

            var categories = new HashSet<string>();
            if (parser(elem, "properties").Any())
            {
                var tags = parser(elem, "properties").Elements("property")
                    .Where(c => c.Attribute("name").Value.Equals("Category", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                tags.ForEach(x => categories.Add(x.Attribute("value").Value));
            }

            return categories;
        }

        private void AddSystemInformation(XDocument doc)
        {
            if (doc.Descendants("environment") == null)
                return;

            var env = doc.Descendants("environment").FirstOrDefault();
            if (env == null)
                return;
                
            if (env.Attribute("nunit-version") != null)
                _extent.AddSystemInfo("NUnit Version", env.Attribute("nunit-version").Value);
            _extent.AddSystemInfo("OS Version", env.Attribute("os-version").Value);
            _extent.AddSystemInfo("Platform", env.Attribute("platform").Value);
            _extent.AddSystemInfo("CLR Version", env.Attribute("clr-version").Value);
            _extent.AddSystemInfo("Machine Name", env.Attribute("machine-name").Value);
            _extent.AddSystemInfo("User", env.Attribute("user").Value);
            _extent.AddSystemInfo("User Domain", env.Attribute("user-domain").Value);
        }
    }
}
