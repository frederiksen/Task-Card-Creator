// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace TaskCardCreator
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      Logger.SetLogWriter(new LogWriterFactory().Create());
      Logger.Write("Application start");
    }

    /// <summary>
    /// Unhandled exception are logged, and not handled
    /// </summary>
    void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      Logger.Write(string.Format("App_DispatcherUnhandledException. Exception: {0}. Stack Trace: {1}", e.Exception.Message, e.Exception.StackTrace));
      e.Handled = false;
    }
  }
}
