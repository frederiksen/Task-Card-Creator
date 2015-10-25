// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls;
using ReportInterface;
using TaskServerServiceInterface;

namespace JIRAServices
{
  public class JiraProject : ITaskProject
  {
    internal JiraUserControl uc;

    public UserControl CreateUserControl(IEnumerable<IReport> supportedReports, IEnumerable<IReport> allReports)
    {
      uc = new JiraUserControl(supportedReports, allReports);
      return uc;
    }

    public IReport SelectedReport => uc.SelectedReport;

    public IEnumerable<string> WorkItemTypeCollection => new List<string>();

    public List<ReportItem> WorkItems
    {
      get
      {
        var l = new List<ReportItem>();
        foreach (var issue in uc.Issues)
        {
          var ri = new ReportItem() { Id = issue.Key, Title = issue.Description, Type = "JIRA Issue" };
          l.Add(ri);
        }
        return l;
      }
    }
  }
}