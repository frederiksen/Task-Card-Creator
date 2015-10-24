// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using Microsoft.TeamFoundation.Client;

namespace TeamFoundationServer2015Services
{
  class PrerequisitesChecker
  {
    /// <summary>
    /// Check if the required TFS Object Model is installed
    /// Throws an exception if missing
    /// </summary>
    public static void CheckPrerequisites()
    {
      using (var tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
      {
      }
    }
  }
}
