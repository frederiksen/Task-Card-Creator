// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumCompact
{
  public class TaskCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public TaskCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
