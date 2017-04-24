// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Text.RegularExpressions;
using System.Web;

namespace JIRAServices
{
    /// <summary>
    /// Methods to remove HTML from strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(this string source)
        {
            var decodedHtml = HttpUtility.HtmlDecode(source);
            var newlinesAdded = decodedHtml.Replace("<BR>", Environment.NewLine);
            newlinesAdded = newlinesAdded.Replace("<br>", Environment.NewLine);
            newlinesAdded = newlinesAdded.Replace("</P>", Environment.NewLine);
            newlinesAdded = newlinesAdded.Replace("</p>", Environment.NewLine);
            return Regex.Replace(newlinesAdded, @"<[^>]+>|", "");
        }

        public static string NullAsEmpty(this string source)
        {
            return source ?? string.Empty;
        }
    }
}