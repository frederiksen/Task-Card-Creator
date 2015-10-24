// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using ReportInterface;

namespace ScrumCompact
{
  public class ImpedimentCardRow
  {
    public ReportItem WorkItem { get; private set; }

    public ImpedimentCardRow(ReportItem workItem)
    {
      WorkItem = workItem;
    }
  }
}
