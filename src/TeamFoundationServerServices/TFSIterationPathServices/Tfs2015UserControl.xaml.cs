// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using ReportInterface;

namespace TeamFoundationServer2015Services
{
  /// <summary>
  /// Interaction logic for Tfs2015.xaml
  /// </summary>
  public partial class Tfs2015UserControl : UserControl, INotifyPropertyChanged
  {
    #region Private fields

    private string projectName;
    private BackgroundWorker worker;
    private string pendingRequest = string.Empty;
    private IReport selectedReport;
    private string selectedIterationPath;
    private TeamConfiguration selectedTeam;

    #endregion

    public ObservableCollection<WorkItem> WorkItems { get; set; }
    public ObservableCollection<IReport> Reports { get; set; }
    public WorkItemStore workItemStoreService { get; set; }

    public IEnumerable<WorkItem> SelectedWorkItems {
      get
      {
        return listView.SelectedItems.Cast<WorkItem>();
      }
    }

    public ObservableCollection<TeamConfiguration> Teams { get; set; }

    public IReport SelectedReport
    {
      get { return selectedReport; }
      set
      {
        if (selectedReport != value)
        {
          selectedReport = value;
          OnPropertyChanged("SelectedReport");
        }
      }
    }

    public TeamConfiguration SelectedTeam
    {
      get { return selectedTeam; }
      set
      {
        if (selectedTeam != value)
        {
          selectedTeam = value;
          OnPropertyChanged("SelectedTeam");
          SelectedIterationPath = value.TeamSettings.CurrentIterationPath;
        }
      }
    }

    public string SelectedIterationPath
    {
      get { return selectedIterationPath; }
      set
      {
        if (selectedIterationPath != value)
        {
          selectedIterationPath = value;
          OnPropertyChanged("SelectedIterationPath");
          QueriesSelectionChanged(value);
        }
      }
    }

    public Tfs2015UserControl(IEnumerable<IReport> reports)
    {
      DataContext = this;

      WorkItems = new ObservableCollection<WorkItem>();
      Reports = new ObservableCollection<IReport>(reports);
      Teams = new ObservableCollection<TeamConfiguration>();

      SelectedReport = Reports.First();

      InitializeComponent();
    }

    private void QueriesSelectionChanged(string queryString)
    {
      //      Logger.Write("Entering QueriesSelectionChanged method");

      progress.Visibility = Visibility.Visible;

      WorkItems.Clear();

      if (worker == null)
      {
        worker = new BackgroundWorker();
        worker.WorkerSupportsCancellation = false;
        worker.WorkerReportsProgress = false;

        worker.DoWork += BwDoWork;
        worker.RunWorkerCompleted += BwRunWorkerCompleted;
      }

      if (worker.IsBusy)
      {
        pendingRequest = queryString;
      }
      else
      {
        worker.RunWorkerAsync(queryString);
      }
    }

    private void BwDoWork(object sender, DoWorkEventArgs e)
    {
      //      Logger.Write("Entering BwDoWork method");

      var newWorkItems = new ObservableCollection<WorkItem>();

      if (!string.IsNullOrEmpty((string) e.Argument))
      {
        var queryString = string.Format("SELECT * FROM WorkItems WHERE [System.IterationPath] = '{0}'", e.Argument);

        var q = new Query(workItemStoreService, queryString);
        if (q.IsLinkQuery)
        {
          var queryResults = q.RunLinkQuery();
          foreach (WorkItemLinkInfo i in queryResults)
          {
            var wi = workItemStoreService.GetWorkItem(i.TargetId);
            newWorkItems.Add(wi);
          }
        }
        else
        {
          var queryResults = q.RunQuery();
          foreach (WorkItem workitem in queryResults)
          {
            newWorkItems.Add(workitem);
          }
        }
      }
      e.Result = newWorkItems;
    }

    private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //      Logger.Write("Entering BwRunWorkerCompleted method");

      if (e.Cancelled == true)
      {
      }
      else if (e.Error != null)
      {
        //       Logger.Write(string.Format("Exception: {0}", e.Error.Message));
//        MessageBox.Show(string.Format("Error: {0}", e.Error.Message));
      }
      else
      {
        var newWorkItems = (ObservableCollection<WorkItem>) e.Result;
        //       Logger.Write(string.Format("Work items loaded: {0}", newWorkItems.Count));
        foreach (var item in newWorkItems)
        {
          WorkItems.Add(item);
        }

        listView.SelectAll();
      }
      progress.Visibility = Visibility.Collapsed;

      // Any pending requests?
      if (!string.IsNullOrEmpty(pendingRequest))
      {
        QueriesSelectionChanged((string) pendingRequest.Clone());
        pendingRequest = string.Empty;
      }
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
