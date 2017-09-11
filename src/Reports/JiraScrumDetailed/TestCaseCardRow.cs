// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using ReportInterface;

namespace JiraScrumDetailed
{
  public class TestCaseCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public TestCaseCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
