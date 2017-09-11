// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumIndexCard3x5
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
