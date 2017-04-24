// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace JiraScrumBasic
{
  public class EpicCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public EpicCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
