// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumBasic
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
