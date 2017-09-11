// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.ComponentModel.Composition;
using System.Windows;
using TaskServerServiceInterface;

namespace DemoServices
{
  [Export(typeof(ITaskServerService))]
  public class Demo : ITaskServerService
  {
    public string Name => "Demo data";
    public string Description => "Demo data";
    public string ShortDescription => "Dev demo data";
    public bool IsInstalled => true;

    public ITaskProject ConnectToProject(Window window)
    {
      return new DemoProject();
    }
  }
}
