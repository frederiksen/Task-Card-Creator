// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDefinedReport
{
  public static class StringExtensions
  {
    public static string DashIfEmpty(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return "-";
      return text;
    }
  }
}
