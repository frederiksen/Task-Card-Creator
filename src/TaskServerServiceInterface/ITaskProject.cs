// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls;
using ReportInterface;

namespace TaskServerServiceInterface
{
  public interface ITaskProject
  {
    UserControl CreateUserControl(IEnumerable<IReport> supportedReports, IEnumerable<IReport> allReports);
    IEnumerable<string> WorkItemTypeCollection { get; }
    List<ReportItem> WorkItems { get; }
    IReport SelectedReport { get; }
  }
}