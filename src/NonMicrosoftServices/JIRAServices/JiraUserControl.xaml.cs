// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using JIRC;
using JIRC.Domain;
using ReportInterface;

namespace JIRAServices
{
  /// <summary>
  /// Interaction logic for JiraUserControl.xaml
  /// </summary>
  public partial class JiraUserControl : UserControl, INotifyPropertyChanged
  {
    private SearchResult searchResult;
    private IEnumerable<IReport> supportedReports;
    private IEnumerable<IReport> allReports;


    public ObservableCollection<IReport> Reports { get; set; }

    private IReport selectedReport;
    public IReport SelectedReport
    {
      get { return selectedReport; }
      set
      {
        selectedReport = value;
        OnPropertyChanged("SelectedReport");
      }
    }
    public IEnumerable<Issue> Issues {
      get
      {
        if (searchResult != null)
          return searchResult.Issues;
        return new List<Issue>();
      } 
    }

    public ObservableCollection<int> Projects { get; set; }

    private string projectUrl;
    public string ProjectUrl
    {
      get { return projectUrl; }
      set
      {
        projectUrl = value;
        OnPropertyChanged("ProjectUrl");
      }
    }

    private string user;
    public string User
    {
      get { return user; }
      set
      {
        user = value;
        OnPropertyChanged("User");
      }
    }

    private string password;
    public string Password
    {
      get { return password; }
      set
      {
        password = value;
        OnPropertyChanged("Password");
      }
    }

    private string jql;
    public string Jql
    {
      get { return jql; }
      set
      {
        jql = value;
        OnPropertyChanged("Jql");
      }
    }

    private string status = "-";
    public string Status
    {
      get { return status; }
      set
      {
        status = value;
        OnPropertyChanged("Status");
      }
    }

    private bool showAll = false;
    public bool ShowAll
    {
      get { return showAll; }
      set
      {
        showAll = value;

        Reports.Clear();
        if (showAll)
        {
          foreach (var report in allReports)
          {
            Reports.Add(report);
          }          
        }
        else
        {
          foreach (var report in supportedReports)
          {
            Reports.Add(report);
          }
        }
        SelectedReport = Reports.FirstOrDefault();

        OnPropertyChanged("ShowAll");
      }
    }


    public JiraUserControl(IEnumerable<IReport> supportedReports, IEnumerable<IReport> allReports)
    {
      DataContext = this;

      this.supportedReports = supportedReports;
      this.allReports = allReports;

      ProjectUrl = "https://jira.atlassian.com";
      Jql = "project = DEMO";
      Reports = new ObservableCollection<IReport>(supportedReports);
      SelectedReport = Reports.First();

      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
      // Load in a seperate thread
      Status = "Loading...";
      Task.Factory.StartNew(() =>
      {
        IJiraRestClient jiraClient;
        if (string.IsNullOrEmpty(User) && string.IsNullOrEmpty(Password))
        {
          jiraClient = JiraRestClientFactory.CreateWithAnonymous(new Uri(ProjectUrl));
        }
        else
        {
          jiraClient = JiraRestClientFactory.CreateWithBasicHttpAuth(new Uri(ProjectUrl), User, Password);
        }

        searchResult = jiraClient.SearchClient.SearchJql(Jql);
      })
        .ContinueWith(ui =>
        {
          if (ui.Status == TaskStatus.Faulted)
          {
            Status = string.Format("Error: {0}", ui.Exception.Message);
          }
          else
          {
            Status = string.Format("Done. Read {0} issues", searchResult.Issues.Count());
          }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
  }
}