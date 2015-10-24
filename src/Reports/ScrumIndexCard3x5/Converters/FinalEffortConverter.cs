// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;
using ReportInterface;

namespace ScrumIndexCard3x5.Converters
{
  class FinalEffortConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var workItem = value as ReportItem;
        if (workItem != null)
        {
          const string fieldName = "Final Effort";
          object finalestimate = workItem.Fields[fieldName];
          if (finalestimate != null)
          {
            var finalestimateString = workItem.Fields[fieldName].ToString();
            switch (finalestimateString)
            {
              case "1":
                return "XS";
              case "2":
                return "S";
              case "3":
                return "M";
              case "5":
                return "L";
              case "8":
                return "XL";
              case "13":
                return "XXL";
              default:
                return "?";
            }
          }
        }
        return "Error: Final Effort";
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