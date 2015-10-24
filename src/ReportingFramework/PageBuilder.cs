// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ReportingFramework
{
  public class PageBuilder
  {
    private readonly PageContent _page;
    private readonly FixedPage _fixedPage;
    private readonly Repeater _repeater;

    public PageBuilder(double width, double height, int marginsLeft, int marginsTop, int marginsRight, int marginsBottom, ContentControl frame)
    {
      _page = new PageContent();
      _fixedPage = new FixedPage {Background = Brushes.White, Width = width, Height = height};

      _repeater = new Repeater();
      var repeatContainer = new Grid {Margin = new Thickness(marginsLeft, marginsTop, marginsRight, marginsBottom)};
      repeatContainer.Children.Add(_repeater);

      frame.SetValue(FixedPage.LeftProperty, 0.00);
      frame.SetValue(FixedPage.TopProperty, 0.00);
      frame.SetValue(FrameworkElement.WidthProperty, _fixedPage.Width);
      frame.SetValue(FrameworkElement.HeightProperty, _fixedPage.Height);

      _fixedPage.Children.Add(frame);
      ((IAddChild)_page).AddChild(_fixedPage);

      frame.Content = repeatContainer;

      frame.Measure(new Size(width, height));
      frame.Arrange(new Rect(0, 0, width, height));

      _repeater.Width = repeatContainer.ActualWidth;
      _repeater.Height = repeatContainer.ActualHeight;
    }

    public bool TryAdd(FrameworkElement control)
    {
      return _repeater.TryAdd(control);
    }

    public PageContent Build()
    {
      return _page;
    }
  }
}
