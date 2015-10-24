// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace MSFforAgile
{
  public class UnknownCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public UnknownCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
