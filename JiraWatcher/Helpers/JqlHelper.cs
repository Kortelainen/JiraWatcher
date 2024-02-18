using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace JiraWatcher.Helpers
{
    internal static class JqlHelper
    {
        internal static string ValidateAndEncodeJql(string jql)
        {
            if (!IsValidJql(jql))
            {
                throw new ArgumentException("Invalid JQL");
            }
            string encodedJql = HttpUtility.UrlEncode(jql).Replace("+", "%20");
            return encodedJql;
        }

        internal static bool IsValidJql(string jql)
        {
            return !string.IsNullOrEmpty(jql) && IsSafe(jql);
        }

        private static bool IsSafe(string url)
        {
            string scriptPattern = @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>";
            Regex regex = new Regex(scriptPattern, RegexOptions.IgnoreCase);
            return !regex.IsMatch(url);
        }
    }
}
