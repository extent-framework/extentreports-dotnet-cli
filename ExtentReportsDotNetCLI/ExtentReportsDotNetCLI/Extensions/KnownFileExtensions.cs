using AventStack.ExtentReports.CLI.Model;

using System;

namespace AventStack.ExtentReports.CLI.Extensions
{
    internal static class KnownFileExtensions
    {
        internal static string GetExtension(TestFramework parserName)
        {
            switch (parserName)
            {
                case TestFramework.NUnit:
                    return "xml";
                default:
                    throw new ArgumentException("Invalid ParserName specified: " + parserName);
            }
        }
    }
}
