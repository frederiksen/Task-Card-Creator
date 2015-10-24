// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
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
    public string Name { get { return "Team Foundation Server"; } }

    public string ShortDescription { get { return "Using team/iteration path"; } }

    public string Description { get { return "Select this if you want to connect to a Team Foundation Server (2015) and select workitems based on team and iteration path. You need to have either Visual Studio 2015 or Team Foundation Server 2015 Object Model installed."; } }

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