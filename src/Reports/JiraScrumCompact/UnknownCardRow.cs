// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace JiraScrumCompact
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
