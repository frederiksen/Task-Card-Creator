// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
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
  public abstract class ReportFromTemplate : ReportBase
  {
    public static readonly DependencyProperty ResourcesProperty = DependencyProperty.Register("Resources", typeof(ResourceDictionary), typeof(ReportFromTemplate), new UIPropertyMetadata(null));

    public ResourceDictionary Resources
    {
      get { return (ResourceDictionary)GetValue(ResourcesProperty); }
      set { SetValue(ResourcesProperty, value); }
    }

    protected ReportFromTemplate()
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

    private FrameworkElement CreateFromDataTemplate(object record)
    {
      return CreateFromDataTemplate(null, record);
    }

    private FrameworkElement CreateFromDataTemplate(string templateName, object dataContext)
    {
      var key = string.IsNullOrEmpty(templateName) ? new DataTemplateKey(dataContext.GetType()) : (object)templateName;
      var control = new ContentPresenter
                      {
                        ContentTemplate = (DataTemplate) Resources[key],
                        Content = dataContext
                      };
      return control;
    }

    private ContentControl CreateFromControlTemplate(string templateName, object dataContext)
    {
      var control = new ContentControl
                      {
                        Template = (ControlTemplate) Resources[templateName],
                        DataContext = dataContext
                      };
      return control;
    }

    private PageBuilder CreatePage(object frameDataContext, Size paperSize, Margins margins)
    {
      var frame = CreateFromControlTemplate("Report.Frame", frameDataContext);
      return new PageBuilder(DPI * paperSize.Width, DPI * paperSize.Height, margins.Left, margins.Top, margins.Right, margins.Bottom, frame);
    }

    private IEnumerable<PageContent> CreatePages(Func<int, object> frameDataContext, Size paperSize, Margins margins, IEnumerable records)
    {
      var pageNumber = 1;
      var currentPage = CreatePage(frameDataContext(pageNumber), paperSize, margins);

      foreach (var record in records)
      {
        var recordContent = CreateFromDataTemplate(record);
        var added = currentPage.TryAdd(recordContent);
        if (!added)
        {
          yield return currentPage.Build();

          pageNumber++;
          currentPage = CreatePage(frameDataContext(pageNumber), paperSize, margins);
          currentPage.TryAdd(recordContent);
        }
      }

      yield return currentPage.Build();
    }
  }
}
