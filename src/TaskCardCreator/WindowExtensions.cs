// This source is subject to Microsoft Public License (Ms-PL).
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace TaskCardCreator
{
  /// <summary>
  /// http://stackoverflow.com/a/339635/600559
  /// </summary>
  internal static class WindowExtensions
  {
    // from winuser.h
    private const int GWL_STYLE = -16,
                      WS_MAXIMIZEBOX = 0x10000,
                      WS_MINIMIZEBOX = 0x20000;

    [DllImport("user32.dll")]
    extern private static int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

    internal static void HideMinimizeAndMaximizeButtons(this Window window)
    {
      var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
      var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

      SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX));
    }
  }
}