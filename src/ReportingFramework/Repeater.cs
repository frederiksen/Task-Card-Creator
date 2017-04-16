// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace ReportingFramework
{
  public class Repeater : WrapPanel
  {
    public bool TryAdd(FrameworkElement control)
    {
      control.Measure(new Size(Width, Height));
      Children.Add(control);

      var sizeAfterControlHasBeenAdded = MeasureOverride(new Size(Width, Height));

      // If the new size is larger than the WrapPanel remove the control and return false
      if (sizeAfterControlHasBeenAdded.Width > Width || sizeAfterControlHasBeenAdded.Height > Height)
      {
        Children.Remove(control);
        return false;
      }
      return true;
    }
  }
}
