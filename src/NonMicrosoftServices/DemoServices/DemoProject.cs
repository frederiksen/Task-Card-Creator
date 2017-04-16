// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ReportInterface;
using TaskServerServiceInterface;

namespace DemoServices
{
  public class DemoProject : ITaskProject
  {
    public UserControl CreateUserControl(IEnumerable<IReport> supportedReports, IEnumerable<IReport> allReports)
    {
      SelectedReport = supportedReports.First();

      return new DemoUserControl();
    }

    public IEnumerable<string> WorkItemTypeCollection { get; private set; }

    public List<ReportItem> WorkItems
    {
      get
      {
        var l = new List<ReportItem>();
        for (int i = 0; i < 250; i++)
        {
          l.Add(new ReportItem()
          {
            Title = string.Format("Title {0}", i),
            Id = i.ToString(),
            Type = "Issue",
          });
        }
        return l;
      }
    }

    public IReport SelectedReport { get; private set; }
  }
}
