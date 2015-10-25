// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.ComponentModel.Composition;
using TaskServerServiceInterface;

namespace TFSQueryServices
{
  [Export(typeof(ITaskServerService))]
  public class Tfs : ITaskServerService
  {
    public string Name => "Team Foundation Server or Visual Studio Online";

    public string ShortDescription => "Using work-item queries";

    public string Description => "Select this provider, if you use queries to select work-items.";

    public bool IsInstalled
    {
      get
      {
        return true;
      }
    }

    public ITaskProject ConnectToProject(Window window)
    {
      using (var tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
      {
        var windowWrapper = new WindowWrapper(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        var result = tpp.ShowDialog(windowWrapper);
        if (result == DialogResult.OK)
        {
          var tfs2010Project = new TfsProject();
          tfs2010Project.projInfo = tpp.SelectedProjects[0];
          tfs2010Project.workItemStoreService = tpp.SelectedTeamProjectCollection.GetService<WorkItemStore>();
          // Get work item types
          tfs2010Project.wiTypes = tfs2010Project.workItemStoreService.Projects[tfs2010Project.projInfo.Name].WorkItemTypes;
          return tfs2010Project;
        }
      }
      return null;
    }
  }
}