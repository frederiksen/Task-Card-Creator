// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace JiraScrumIndexCard3x5
{
  public class NonProductBacklogItemCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public NonProductBacklogItemCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
