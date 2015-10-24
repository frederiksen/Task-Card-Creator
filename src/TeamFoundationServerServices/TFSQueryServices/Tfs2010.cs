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

namespace TeamFoundationServer2010Services
{
  [Export(typeof(ITaskServerService))]
  public class Tfs2012 : ITaskServerService
  {
    public string Name { get { return "Team Foundation Server"; } }

    public string ShortDescription { get { return "Using work-item queries"; } }

    public string Description { get { return "Select this if you want to connect to a Team Foundation Server (2010/2012/2013) and select workitems based on queries. You need to have either Visual Studio 2010 or Team Foundation Server 2010 Object Model installed."; } }

    public bool IsInstalled
    {
      get
      {
        try
        {
          PrerequisitesChecker.CheckPrerequisites();
          return true;
        }
        catch (Exception)
        {
        }
        return false;
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
          var tfs2010Project = new Tfs2010Project();
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
