using AventStack.ExtentReports;

namespace AventStack.ExtentReports.CLI.Extensions
{
    internal static class StatusExtensions
    {
        /// <summary>
        /// Convert a string into enum Status
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static Status ToStatus(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                //return Status.Unknown;
            }

            str = str.Trim().ToLower();

            switch (str)
            {
                case "skipped":
                case "ignored":
                case "not-run":
                case "notrun":
                case "notexecuted":
                case "not-executed":
                    return Status.Skip;

                case "pass":
                case "passed":
                case "success":
                    return Status.Pass;

                case "warning":
                case "bad":
                case "pending":
                case "inconclusive":
                case "notrunnable":
                case "disconnected":
                case "passedbutrunaborted":
                    return Status.Warning;

                case "fail":
                case "failed":
                case "failure":
                case "invalid":
                    return Status.Fail;

                case "error":
                case "aborted":
                case "timeout":
                    return Status.Error;

                default:
                    //return Status.Unknown;
                    return Status.Debug;
            }
        }
    }
}
