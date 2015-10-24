// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumDetailed
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
