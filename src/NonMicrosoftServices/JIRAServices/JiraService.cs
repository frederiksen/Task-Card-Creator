// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.ComponentModel.Composition;
using System.Windows;
using TaskServerServiceInterface;

namespace JIRAServices
{
  [Export(typeof(ITaskServerService))]
  public class JiraService : ITaskServerService
  {
    public string Name => "Atlassian JIRA";
    public string Description => "Experimental implementation";
    public string ShortDescription => "Experimental implementation";
    public bool IsInstalled => true;

    public ITaskProject ConnectToProject(Window window)
    {
      return new JiraProject();
    }
  }
}
