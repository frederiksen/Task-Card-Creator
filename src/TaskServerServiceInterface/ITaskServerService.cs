// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Windows;

namespace TaskServerServiceInterface
{
  public interface ITaskServerService
  {
    string Name { get; }
    string Description { get; }
    string ShortDescription { get; }
    bool IsInstalled { get; }
    ITaskProject ConnectToProject(Window window);
  }
}