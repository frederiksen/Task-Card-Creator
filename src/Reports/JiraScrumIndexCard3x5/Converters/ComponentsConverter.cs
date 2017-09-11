// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using ReportInterface;

namespace JiraScrumIndexCard3x5.Converters
{
  internal class ComponentsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var workItem = value as ReportItem;
        if (workItem != null)
        {
          const string fieldName = "Components";
          if (workItem.Fields.ContainsKey(fieldName) && workItem.Fields[fieldName] != null)
          {
              IEnumerable<object> workItemField = (IEnumerable<object>)workItem.Fields[fieldName];
              return string.Join(", ", workItemField);
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
