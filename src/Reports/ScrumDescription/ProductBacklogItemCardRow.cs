// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumDescription
{
  public class ProductBacklogItemCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public ProductBacklogItemCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
