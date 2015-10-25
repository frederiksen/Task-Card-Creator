// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Text.RegularExpressions;
using System.Web;

namespace TFSQueryServices
{
  /// <summary>
  /// Methods to remove HTML from strings.
  /// </summary>
  public class HtmlRemoval
  {
    /// <summary>
    /// Remove HTML from string with Regex.
    /// </summary>
    public static string StripTagsRegex(string source)
    {
      var decodedHtml = HttpUtility.HtmlDecode(source);
      var newlinesAdded = decodedHtml.Replace("<BR>", Environment.NewLine);
      newlinesAdded = newlinesAdded.Replace("<br>", Environment.NewLine);
      newlinesAdded = newlinesAdded.Replace("</P>", Environment.NewLine);
      newlinesAdded = newlinesAdded.Replace("</p>", Environment.NewLine);
      return Regex.Replace(newlinesAdded, @"<[^>]+>|", "");
    }
  }
}