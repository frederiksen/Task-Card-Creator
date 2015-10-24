// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace MSFforAgileBasic
{
  public class BugCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public BugCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
