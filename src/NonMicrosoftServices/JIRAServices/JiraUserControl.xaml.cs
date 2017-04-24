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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using JIRAServices.Properties;
using ReportInterface;

namespace JIRAServices
{
    /// <summary>
    /// Interaction logic for JiraUserControl.xaml
    /// </summary>
    public partial class JiraUserControl : UserControl, INotifyPropertyChanged
    {
        private IPagedQueryResult<Issue> searchResult;
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
        public ObservableCollection<Issue> Issues { get; set; }

        public IEnumerable<Issue> SelectedIssues => this.listView.SelectedItems.Cast<Issue>();

        public ObservableCollection<int> Projects { get; set; }

        public string ProjectUrl
        {
            get { return Settings.Default.JiraService_Url; }
            set
            {
                Settings.Default.JiraService_Url = value;
                OnPropertyChanged("ProjectUrl");
            }
        }

        public string User
        {
            get { return Settings.Default.JiraService_User; }
            set
            {
                Settings.Default.JiraService_User = value;
                OnPropertyChanged("User");
            }
        }

        public string Jql
        {
            get { return Settings.Default.JiraService_Jql; }
            set
            {
                Settings.Default.JiraService_Jql = value;
                OnPropertyChanged("Jql");
            }
        }

        private string pageInfo = "-";
        public string PageInfo
        {
            get { return pageInfo; }
            set
            {
                pageInfo = value;
                OnPropertyChanged("PageInfo");
            }
        }

        public ObservableCollection<int> AvailableItemsPerPage { get; private set; }

        public int ItemsPerPage
        {
            get
            {
                return Settings.Default.JiraService_Paging_ItemsPerPage;
            }

            set
            {
                Settings.Default.JiraService_Paging_ItemsPerPage = value;
                OnPropertyChanged("ItemsPerPage");
            }
        }

        private bool isNavigatingBackEnabled;
        public bool IsNavigatingBackEnabled
        {
            get
            {
                return this.isNavigatingBackEnabled;
            }

            set
            {
                this.isNavigatingBackEnabled = value;
                OnPropertyChanged("IsNavigatingBackEnabled");
            }
        }

        private bool isNavigatingNextEnabled;
        public bool IsNavigatingNextEnabled
        {
            get
            {
                return this.isNavigatingNextEnabled;
            }

            set
            {
                this.isNavigatingNextEnabled = value;
                OnPropertyChanged("IsNavigatingNextEnabled");
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                OnPropertyChanged("IsLoading");
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

            Reports = new ObservableCollection<IReport>(supportedReports);
            Issues = new ObservableCollection<Issue>();
            AvailableItemsPerPage = new ObservableCollection<int> { 10, 20, 50, 100 };
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
            this.LoadIssues(1);
        }

        private void LoadIssues(int page)
        {
            // Load in a seperate thread
            Settings.Default.Save();
            this.Issues.Clear();
            this.PageInfo = "Loading...";
            this.IsLoading = true;
            this.IsNavigatingBackEnabled = false;
            this.IsNavigatingNextEnabled = false;

            Task.Factory.StartNew(() =>
                {
                    Jira jiraClient;
                    if (string.IsNullOrEmpty(this.User) && string.IsNullOrEmpty(this.passwordBox.Password))
                    {
                        jiraClient = Jira.CreateRestClient(this.ProjectUrl);
                    }
                    else
                    {
                        jiraClient = Jira.CreateRestClient(this.ProjectUrl, this.User, this.passwordBox.Password);
                    }

                    int startAt = (page - 1) * ItemsPerPage;
                    this.searchResult = jiraClient.Issues.GetIssuesFromJqlAsync(this.Jql, maxIssues: ItemsPerPage, startAt: startAt).GetAwaiter().GetResult();
                })
                .ContinueWith(ui =>
                {
                    if (ui.Status == TaskStatus.Faulted)
                    {
                        this.PageInfo = string.Format("Error: {0}", ui.Exception.Message);
                    }
                    else
                    {
                        int totalPages = (this.searchResult.TotalItems / this.searchResult.ItemsPerPage) + 1;
                        int currentPage = (this.searchResult.StartAt / this.searchResult.ItemsPerPage) + 1;

                        this.PageInfo = $"{currentPage} of {totalPages}";
                        this.IsNavigatingBackEnabled = currentPage > 1;
                        this.IsNavigatingNextEnabled = currentPage < totalPages;
                        this.IsLoading = false;

                        foreach (var issue in this.searchResult)
                        {
                            this.Issues.Add(issue);
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ButtonFirst_OnClick(object sender, RoutedEventArgs e)
        {
            if (searchResult != null)
            {
                this.LoadIssues(1);
            }
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            if (searchResult != null)
            {
                int currentPage = (this.searchResult.StartAt / this.searchResult.ItemsPerPage) + 1;

                this.LoadIssues(Math.Max(1, currentPage - 1));
            }
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            if (searchResult != null)
            {
                int currentPage = (this.searchResult.StartAt / this.searchResult.ItemsPerPage) + 1;
                int totalPages = (this.searchResult.TotalItems / this.searchResult.ItemsPerPage) + 1;

                this.LoadIssues(Math.Min(currentPage + 1, totalPages));
            }
        }

        private void ButtonLast_OnClick(object sender, RoutedEventArgs e)
        {
            if (searchResult != null)
            {
                int totalPages = (this.searchResult.TotalItems / this.searchResult.ItemsPerPage) + 1;

                this.LoadIssues(Math.Max(1, totalPages));
            }
        }

        private void ComboboxNumberOfRecords_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchResult != null)
            {
                this.LoadIssues(1);
            }
        }
    }
}