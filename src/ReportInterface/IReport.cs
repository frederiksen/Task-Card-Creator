// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Documents;

namespace ReportInterface
{
  public interface IReport
  {
    bool IsSupported(IEnumerable<string> wiTypeCollection);
    string Description { get; }
    Size PaperSize { get; }
    Margins Margins { get; }
    bool TeamCustomized { get; }
    FixedDocument Create(IEnumerable<ReportItem> data);
  }
}
