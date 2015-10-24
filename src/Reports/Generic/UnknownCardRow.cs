// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace Generic
{
  public class UnknownCardRow
  {
    public ReportItem ReportItem { get; private set; }

    public UnknownCardRow(ReportItem reportItem)
    {
      ReportItem = reportItem;
    }
  }
}
