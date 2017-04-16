// This source is subject to Microsoft Public License (Ms-PL).
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

namespace ScrumBasic
{
  /// <summary>
  /// Interaction logic for Template.xaml
  /// </summary>
  [Export(typeof(IReport))]
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
      return wiTypeCollection.Contains("Product Backlog Item");
    }

    public string Description
    {
      get { return "Scrum Basic Report"; }
    }

    public Size PaperSize { get { return new Size(8.27, 11.69); } }

    public Margins Margins
    {
      get
      {
        return new Margins(0, 0, 0, 0);
      }
    }

    public bool TeamCustomized { get { return false; } }

    public FixedDocument Create(IEnumerable<ReportItem> data)
    {
      var rows = new List<object>();
      foreach (var workItem in data)
      {
        if (workItem.Type == "Product Backlog Item")
        {
          rows.Add(new ProductBacklogItemCardRow(workItem));
        }
        else if (workItem.Type == "Task")
        {
          rows.Add(new TaskCardRow(workItem));
        }
        else if (workItem.Type == "Bug")
        {
          rows.Add(new BugCardRow(workItem));
        }
        else if (workItem.Type == "Impediment")
        {
          rows.Add(new ImpedimentCardRow(workItem));
        }
        else
        {
          rows.Add(new UnknownCardRow(workItem));
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

      public bool IsFirstPage { get { return PageNumber == 1; } }
      public int PageNumber { get; set; }
      public DateTime ReportDate { get; set; }
    }
  }
}