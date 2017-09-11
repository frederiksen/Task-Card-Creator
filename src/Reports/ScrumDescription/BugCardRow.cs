// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumDescription
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
