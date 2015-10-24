// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ReportingFramework
{
  public abstract class ReportFromMemory : ReportBase
  {
    public static readonly DependencyProperty ResourcesProperty = DependencyProperty.Register("Resources", typeof(ResourceDictionary), typeof(ReportFromMemory), new UIPropertyMetadata(null));

    public ResourceDictionary Resources
    {
      get { return (ResourceDictionary)GetValue(ResourcesProperty); }
      set { SetValue(ResourcesProperty, value); }
    }

    protected ReportFromMemory()
    {
      Resources = new ResourceDictionary();
    }

    protected FixedDocument GenerateReport(Func<int, object> frameDataContext, Size paperSize, Margins margins, IEnumerable records)
    {
      var document = new FixedDocument();
      document.DocumentPaginator.PageSize = new Size(DPI * paperSize.Width, DPI * paperSize.Height);
      foreach (var page in CreatePages(frameDataContext, paperSize, margins, records))
      {
        document.Pages.Add(page);
      }
      return document;
    }

    private static IEnumerable<PageContent> CreatePages(Func<int, object> frameDataContext, Size paperSize, Margins margins, IEnumerable records)
    {
      var pageNumber = 1;
      var currentPage = CreatePage(frameDataContext(pageNumber), paperSize, margins);

      foreach (var record in records)
      {
        var control = record as FrameworkElement;

        var added = currentPage.TryAdd(control);
        if (!added)
        {
          yield return currentPage.Build();

          pageNumber++;
          currentPage = CreatePage(frameDataContext(pageNumber), paperSize, margins);
          currentPage.TryAdd(control);
        }
      }

      yield return currentPage.Build();
    }

    private static PageBuilder CreatePage(object frameDataContext, Size paperSize, Margins margins)
    {
      var frame = new ContentControl();
      return new PageBuilder(DPI * paperSize.Width, DPI * paperSize.Height, margins.Left, margins.Top, margins.Right, margins.Bottom, frame);
    }
  }
}