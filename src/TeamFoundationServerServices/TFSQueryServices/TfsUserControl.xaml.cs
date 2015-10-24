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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ReportInterface;

namespace TFSQueryServices
{
  /// <summary>
  /// Interaction logic for Tfs2010UserControl
  /// </summary>
  public partial class TfsUserControl : UserControl, INotifyPropertyChanged
  {
    #region Private fields

    private string projectName;
    private BackgroundWorker worker;
    private string pendingRequest = string.Empty;
    private IReport selectedReport;

    #endregion

    #region Public properties

    public ObservableCollection<WorkItem> WorkItems { get; set; }
    public ObservableCollection<IReport> Reports { get; set; }
    public WorkItemStore workItemStoreService { get; set; }
    public IReport SelectedReport
    {
      get { return selectedReport; }
      set { selectedReport = value; OnPropertyChanged("SelectedReport"); }
    }

    public IEnumerable<WorkItem> SelectedWorkItems
    {
      get
      {
        return listView.SelectedItems.Cast<WorkItem>();
      }
    }

    #endregion

    #region Constructors

    public TfsUserControl(IEnumerable<IReport> reports)
    {
      DataContext = this;

      WorkItems = new ObservableCollection<WorkItem>();
      Reports = new ObservableCollection<IReport>(reports);

      SelectedReport = Reports.First();

      InitializeComponent();
    }

    #endregion

    #region Tree handling

    private enum QueryTypes
    {
      Folder,
      MyQ,
      TeamQ,
      FView,
      DView,
      HView,
      None
    }

    private void QueriesSelectionChanged(string queryString)
    {
      //Logger.Write("Entering QueriesSelectionChanged method");

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
      //Logger.Write("Entering BwDoWork method");

      var newWorkItems = new ObservableCollection<WorkItem>();

      if (!string.IsNullOrEmpty((string)e.Argument))
      {
        var q = new Query(workItemStoreService, (string)e.Argument);
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
      //Logger.Write("Entering BwRunWorkerCompleted method");

      if (e.Cancelled == true)
      {
      }
      else if (e.Error != null)
      {
        //Logger.Write(string.Format("Exception: {0}", e.Error.Message));
        MessageBox.Show(string.Format("Error: {0}", e.Error.Message));
      }
      else
      {
        var newWorkItems = (ObservableCollection<WorkItem>)e.Result;
        //Logger.Write(string.Format("Work items loaded: {0}", newWorkItems.Count));
        foreach (var item in newWorkItems)
        {
          WorkItems.Add(item);
        }

        listView.SelectAll();
      }
      progress.Visibility = Visibility.Collapsed;

      // Any pending requests?
      if (pendingRequest != string.Empty)
      {
        QueriesSelectionChanged((string)pendingRequest.Clone());
        pendingRequest = string.Empty;
      }
    }

    public void BuildQueryTree(QueryHierarchy queryHierarchy, string project)
    {
      projectName = project;
      BuildQueryHierarchy(queryHierarchy, project);
    }

    private void BuildQueryHierarchy(QueryHierarchy QueryHierarchy, string project)
    {
      var rootContent = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(4) };
      var lbl = new TextBlock{ Text = project, FontSize = 15 };
      rootContent.Children.Add(lbl);

      var root = new TreeViewItem {IsExpanded = true, Header = rootContent};
      foreach (QueryFolder query in QueryHierarchy)
      {
        DefineFolder(query, root);
      }
      Queries.Items.Add(root);
    }

    private void DefineFolder(QueryFolder query, TreeViewItem father)
    {
      var item = new TreeViewItem {IsExpanded = true};
      var type = QueryTypes.Folder;
      if (query.IsPersonal) type = QueryTypes.MyQ;
      else if (query.Name == "Team Queries") type = QueryTypes.TeamQ;
      item.Header = CreateTreeItem(query.Name, type);
      father.Items.Add(item);
      foreach (QueryItem sub_query in query)
      {
        if (sub_query.GetType() == typeof(QueryFolder))
          DefineFolder((QueryFolder)sub_query, item);
        else
          DefineQuery((QueryDefinition)sub_query, item);
      }
    }

    private static StackPanel CreateTreeItem(string value, QueryTypes type)
    {
      var stake = new StackPanel {Orientation = Orientation.Horizontal, Margin=new Thickness(2, 2, 0, 2)};
      var img = new Image
                    {
                      Stretch = System.Windows.Media.Stretch.Uniform,
                      Source = GetImage(type),
                      Margin = new Thickness(0, 0, 5, 0)
                    };
      var lbl = new TextBlock {Text = value};
      stake.Children.Add(img);
      stake.Children.Add(lbl);
      return stake;
    }

    private void DefineQuery(QueryDefinition query, TreeViewItem QueryFolder)
    {
      var item = new TreeViewItem {IsExpanded = true};
      QueryTypes type;
      switch (query.QueryType)
      {
        case QueryType.List: type = QueryTypes.FView; break;
        case QueryType.OneHop: type = QueryTypes.DView; break;
        case QueryType.Tree: type = QueryTypes.HView; break;
        default: type = QueryTypes.None; break;
      }
      item.Header = CreateTreeItem(query.Name, type);
      item.Tag = query;
      QueryFolder.Items.Add(item);
    }

    private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      var query = string.Empty;
      var item = ((TreeViewItem)((TreeView)sender).SelectedItem);
      if (item != null && item.Tag != null)
      {
        query = ((QueryDefinition)item.Tag).QueryText;
        query = query.Replace("@project", "'" + projectName + "'");

      }
      QueriesSelectionChanged(query);
    }

    static BitmapSource GetImage(QueryTypes type)
    {
      switch (type)
      {
        case QueryTypes.MyQ:
          return DisplayImage.GetImageSource(Properties.Resources.MyQ);
        case QueryTypes.TeamQ:
          return DisplayImage.GetImageSource(Properties.Resources.TeamQ);
        case QueryTypes.Folder:
          return DisplayImage.GetImageSource(Properties.Resources.Folder);
        case QueryTypes.FView:
          return DisplayImage.GetImageSource(Properties.Resources.FView);
        case QueryTypes.DView:
          return DisplayImage.GetImageSource(Properties.Resources.DView);
        case QueryTypes.HView:
          return DisplayImage.GetImageSource(Properties.Resources.HView);
        default:
          return null;
      }
    }

    #endregion

    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }
  }
}
