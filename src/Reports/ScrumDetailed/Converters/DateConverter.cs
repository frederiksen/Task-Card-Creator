// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;
using ReportInterface;

namespace ScrumDetailed.Converters
{
  internal class StartConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var workItem = value as ReportItem;
        if (workItem != null)
        {
          const string fieldName = "Start Date";
          if (workItem.Fields[fieldName] != null)
          {
            DateTime fieldValue = (DateTime) workItem.Fields[fieldName];
            string date = string.Empty;
            date += fieldValue.Month.ToString();
            date += "/";
            date += fieldValue.Day.ToString();
            date += "/";
            date += fieldValue.Year.ToString();
            return date;
          }
          return "-";
        }
        return "Error: Incorrect type";
      }
      catch (Exception exception)
      {
        return string.Format("Error: {0}", exception.Message);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
