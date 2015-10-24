// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;

namespace TeamFoundationServer2010Services
{
  /// <summary>
  /// http://stackoverflow.com/a/10296513/600559
  /// </summary>
  public class WindowWrapper : System.Windows.Forms.IWin32Window
  {
    public WindowWrapper(IntPtr handle)
    {
      hwnd = handle;
    }

    public IntPtr Handle
    {
      get { return hwnd; }
    }

    private IntPtr hwnd;
  }
}
