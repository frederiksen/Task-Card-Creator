// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TaskServerServiceInterface;

namespace TFSIterationPathServices
{
  [Export(typeof(ITaskServerService))]
  public class Tfs : ITaskServerService
  {
    public string Name => "Team Foundation Server or Azure DevOps";

    public string ShortDescription => "Using iteration paths and teams";

    public string Description => "Select this provider, if your workmode has iterations or sprints.";

    public bool IsInstalled => true;

    public ITaskProject ConnectToProject(Window window)
    {
      using (var tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
      {
        var windowWrapper = new WindowWrapper(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        var result = tpp.ShowDialog(windowWrapper);
        if (result == DialogResult.OK)
        {
          var tfs2015Project = new TfsProject();
          tfs2015Project.projInfo = tpp.SelectedProjects[0];
          tfs2015Project.teamConfig = tpp.SelectedTeamProjectCollection.GetService<TeamSettingsConfigurationService>();
          tfs2015Project.workItemStoreService = tpp.SelectedTeamProjectCollection.GetService<WorkItemStore>();
          // Get work item types
          tfs2015Project.wiTypes = tfs2015Project.workItemStoreService.Projects[tfs2015Project.projInfo.Name].WorkItemTypes;
          return tfs2015Project;
        }
      }
      return null;
    }
  }
}
