// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Data;

namespace TFSIterationPathServices.Converters
{
  public class PaperTextConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var size = (Size)value;
      if (size != null)
      {
        return string.Format("{0}\" x {1}\"", size.Width, size.Height);
      }
      return "-";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
