// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Documents;
using ReportingFramework;
using System.Windows;
using ReportInterface;

namespace JiraScrumDetailed
{
  /// <summary>
  /// Interaction logic for Template.xaml
  /// </summary>
  [Export(typeof (IReport))]
  public partial class Template : ReportFromTemplate, IReport
  {
    public Template()
    {
      InitializeComponent();
    }

    public bool IsSupported(IEnumerable<string> wiTypeCollection)
    {
      if (wiTypeCollection == null)
      {
        return false;
      }
      if (wiTypeCollection.Count() == 0)
      {
        return false;
      }
      // Only the Scrum process template has this type
      return wiTypeCollection.Contains("Story");
    }

    public string Description
    {
      get { return "Jira Scrum Detailed Report"; }
    }

    public Size PaperSize
    {
      get { return new Size(8.27, 11.69); }
    }

    public Margins Margins
    {
      get
      {
        return new Margins(0, 0, 0, 0);
      }
    }

    public bool TeamCustomized
    {
      get { return false; }
    }

    public FixedDocument Create(IEnumerable<ReportItem> data)
    {
      var rows = new List<object>();
      foreach (var workItem in data)
      {
        switch (workItem.Type)
        {
          case "Story":
            rows.Add(new StoryCardRow(workItem));
            break;
          case "Task":
            rows.Add(new TaskCardRow(workItem));
            break;
          case "Bug":
            rows.Add(new BugCardRow(workItem));
            break;
          case "Epic":
            rows.Add(new EpicCardRow(workItem));
            break;
          default:
            rows.Add(new UnknownCardRow(workItem));
            break;
        }
      }

      return GenerateReport(
        page => new PageInfo(page, DateTime.Now),
        PaperSize,
        Margins,
        rows
        );
    }

    /// <summary>
    /// This class becomes the data context for every page. It gives the page 
    /// access to the page number.
    /// </summary>
    private class PageInfo
    {
      public PageInfo(int pageNumber, DateTime reportDate)
      {
        PageNumber = pageNumber;
        ReportDate = reportDate;
      }

      public bool IsFirstPage
      {
        get { return PageNumber == 1; }
      }

      public int PageNumber { get; set; }
      public DateTime ReportDate { get; set; }
    }
  }
}