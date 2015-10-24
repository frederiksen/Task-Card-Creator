// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System.ComponentModel.Composition;
using System.Windows;
using TaskServerServiceInterface;

namespace DemoServices
{
  [Export(typeof(ITaskServerService))]
  public class Demo : ITaskServerService
  {
    public string Name { get { return "Demo data"; } }
    public string Description { get { return "Demo data"; } }
    public bool IsInstalled { get { return true; } }
    public ITaskProject ConnectToProject(Window window)
    {
      return new DemoProject();
    }
  }
}
