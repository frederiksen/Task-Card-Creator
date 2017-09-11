// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Navigation;
using TaskServerServiceInterface;

namespace TaskCardCreator
{
  /// <summary>
  /// Interaction logic for TaskServiceProviderWindow.xaml
  /// </summary>
  public partial class TaskServiceProviderWindow : Window, INotifyPropertyChanged
  {
    public ObservableCollection<ITaskServerService> TaskServerServices { get; set; }

    private ITaskServerService selectedTaskServerService;
    public ITaskServerService SelectedTaskServerService
    {
      get { return selectedTaskServerService; }
      set
      {
        selectedTaskServerService = value;
        OnPropertyChanged("SelectedTaskServerService");
      }
    }

    public TaskServiceProviderWindow()
    {
      DataContext = this;

      TaskServerServices = new ObservableCollection<ITaskServerService>();

      InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true; 
      Close();
    }

    private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
