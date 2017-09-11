// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using GoogleAnalyticsTracker.Simple;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using ReportInterface;
using System.Windows.Controls.Ribbon;
using TaskServerServiceInterface;
using System.Xml;

namespace TaskCardCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private int reportNumber = 1;

        [ImportMany(typeof(IReport))]
        private IEnumerable<IReport> reports;

        [ImportMany(typeof(ITaskServerService))]
        private IEnumerable<ITaskServerService> serviceProviders;

        private Dictionary<TabItem, ITaskProject> projects = new Dictionary<TabItem, ITaskProject>();

        private readonly SimpleTracker simpleTracker = new SimpleTracker("UA-55415144-2", "taskcardcreator.codeplex.com");

        public MainWindow()
        {
            LoadReports();

            InitializeComponent();

            simpleTracker.TrackPageViewAsync("Task Card Creator", "");
            var version = Assembly.GetEntryAssembly().GetName().Version;

            simpleTracker.TrackEventAsync("Version", version.ToString());

            this.SourceInitialized += (x, y) => this.HideMinimizeAndMaximizeButtons();

            CheckForNewVersion();
        }

        private void LoadReports()
        {
            Logger.Write("Entering LoadReports method");

            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Adds all the parts found in all assemblies in subfolders
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var path in Directory.EnumerateDirectories(exePath, "*", SearchOption.TopDirectoryOnly))
            {
                AppDomain.CurrentDomain.AppendPrivatePath(path);
                catalog.Catalogs.Add(new DirectoryCatalog(path));
            }

            // Create the CompositionContainer with the parts in the catalog
            var container = new CompositionContainer(catalog);

            // Fill the imports of this object
            container.ComposeParts(this);
        }

        private void SelectProjectButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering SelectProjectButtonClick method");

            var dlg = new TaskServiceProviderWindow { Owner = this };
            foreach (var s in serviceProviders)
            {
                dlg.TaskServerServices.Add(s);
                if (s.IsInstalled && dlg.SelectedTaskServerService == null)
                    dlg.SelectedTaskServerService = s;
            }

            var r = dlg.ShowDialog();
            if (r.HasValue && r.Value)
            {
                var taskServerService = dlg.SelectedTaskServerService;
                if (taskServerService != null)
                {
                    simpleTracker.TrackEventAsync("Service", taskServerService.Name);

                    var project = taskServerService.ConnectToProject(this);

                    if (project != null)
                    {
                        var wiTypes = project.WorkItemTypeCollection;

                        var supportedReports = from rep in reports
                                               where rep.IsSupported(wiTypes)
                                               select rep;

                        // Create tab
                        var uc = project.CreateUserControl(supportedReports, reports);
                        var tab = new TabItem { Header = string.Format("Report #{0}", reportNumber), Content = uc };
                        reportNumber++;
                        TabControl.Items.Insert(1, tab);
                        TabControl.SelectedItem = tab;

                        projects[tab] = project;
                    }
                }
            }
        }

        private void GoToWebSiteButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering GoToWebSiteButtonClick method");
            Process.Start(new ProcessStartInfo(@"https://github.com/frederiksen/Task-Card-Creator"));
        }

        private void CheckForUpdatesButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering CheckForUpdatesButtonClick method");
            Process.Start(new ProcessStartInfo(@"https://github.com/frederiksen/Task-Card-Creator/releases/latest"));
        }

        private void SubmitBugButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering SubmitBugButtonClick method");
            Process.Start(new ProcessStartInfo(@"https://github.com/frederiksen/Task-Card-Creator/issues"));
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering HelpButtonClick method");
            Process.Start(new ProcessStartInfo(@"https://github.com/frederiksen/Task-Card-Creator"));
        }

        private void RibbonLogClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering RibbonLogClick method");
            var folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          "TaskCardCreator");
            Process.Start(folderName);
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering ExitButtonClick method");
            Close();
        }

        private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var isTaskCardTab = (TabControl.SelectedIndex != 0);
            TaskCardContextualTab.Visibility = isTaskCardTab ? Visibility.Visible : Visibility.Collapsed;
            // Change focus to contextual tab?
            if (isTaskCardTab)
            {
                TaskCardTab.IsSelected = true;
            }

            var tab = TabControl.SelectedItem as TabItem;
            if (tab != null)
            {
                var doc = tab.Content as DocumentViewer;
                CreateButton.IsEnabled = (doc == null);
                PrintButton.IsEnabled = (doc != null);
                ZoomIn.IsEnabled = (doc != null);
                ZoomOut.IsEnabled = (doc != null);
            }
        }

        private DocumentViewer GetDocViewer()
        {
            var tab = TabControl.SelectedItem as TabItem;
            if (tab != null)
            {
                var doc = tab.Content as DocumentViewer;
                if (doc != null)
                {
                    return doc;
                }
            }
            return null;
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering CreateButtonClick method");

            var tab = TabControl.SelectedItem as TabItem;
            if (tab != null)
            {
                var reportProperty = tab.Content as UserControl;
                if (reportProperty != null)
                {
                    // Create report
                    IReport reportTemplate = null;
                    IEnumerable<ReportItem> workItems = null;

                    var selectedItem = TabControl.SelectedItem as TabItem;
                    var project = projects[selectedItem];

                    reportTemplate = project.SelectedReport;
                    simpleTracker.TrackEventAsync("Report", reportTemplate.Description);

                    workItems = project.WorkItems;
                    simpleTracker.TrackEventAsync("Workitems", workItems.Count().ToString());

                    var ms = new MemoryStream();
                    var pkg = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
                    var pack = string.Format("pack://{0}.xps", Guid.NewGuid());
                    PackageStore.AddPackage(new Uri(pack), pkg);
                    var compressionOption = CompressionOption.NotCompressed;
                    var document = new XpsDocument(pkg, compressionOption, pack);

                    var report = reportTemplate.Create(workItems);
                    var writer = XpsDocument.CreateXpsDocumentWriter(document);
                    writer.Write(report.DocumentPaginator);

                    // Create doc
                    var doc = new DocumentViewer { Document = document.GetFixedDocumentSequence() };
                    // Remove toolbar from DocumentViewer
                    var contentHost = doc.Template.FindName("PART_ContentHost", doc) as ScrollViewer;
                    if (contentHost != null)
                    {
                        var grid = contentHost.Parent as Grid;
                        if (grid != null)
                        {
                            grid.Children.RemoveAt(0);
                        }
                    }

                    doc.FitToMaxPagesAcross(1);
                    tab.Content = doc;

                    TabSelectionChanged(null, null);
                }
            }
        }

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering PrintButtonClick method");
            var docViewer = GetDocViewer();
            if (docViewer != null)
            {
                docViewer.Print();
            }
        }

        private void ZoomInButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering ZoomInButtonClick method");
            var docViewer = GetDocViewer();
            if (docViewer != null)
            {
                docViewer.IncreaseZoom();
            }
        }

        private void ZoomOutButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Write("Entering ZoomOutButtonClick method");
            var docViewer = GetDocViewer();
            if (docViewer != null)
            {
                docViewer.DecreaseZoom();
            }
        }

        private async void CheckForNewVersion()
        {
            var result = await Task.Run(async () =>
            {
                try
                {
                    var client = new HttpClient();
                    var getStringTask = client.GetStringAsync("https://github.com/frederiksen/Task-Card-Creator/releases.atom");
                    var contents = await getStringTask;

                    DateTime latestReleaseDateTime = new DateTime(1980, 1, 1);
                    string latestTitle = string.Empty;

                    var xml = new XmlDocument();
                    xml.LoadXml(contents);

                    var nsmgr = new XmlNamespaceManager(xml.NameTable);
                    nsmgr.AddNamespace("msb", "http://www.w3.org/2005/Atom");

                    var releases = xml.SelectNodes("/msb:feed/msb:entry", nsmgr);

                    foreach(XmlNode release in releases)
                    {
                        var updated = release["updated"].InnerText;
                        var updatedTime = DateTime.Parse(updated);
                        if (updatedTime > latestReleaseDateTime)
                        {
                            latestReleaseDateTime = updatedTime;
                            latestTitle = release["title"].InnerText;
                        }
                    }

                    return latestTitle;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            });
            if (!string.IsNullOrEmpty(result) && result != "Task Card Creator 8.0")
            {
                var text = new TextBlock() { Margin = new Thickness(15), Text = "There is a new version available for download" };
                IntroTab.Content = text;
            }
        }
    }
}