// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System.ComponentModel.Composition;
using System.Windows;
using TaskServerServiceInterface;

namespace JIRAServices
{
  [Export(typeof(ITaskServerService))]
  public class Jira : ITaskServerService
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
