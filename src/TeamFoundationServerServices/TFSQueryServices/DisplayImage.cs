// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://taskcardcreator.codeplex.com for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace TeamFoundationServer2010Services
{
  public static class DisplayImage
  {
    public static BitmapSource GetImageSource(Bitmap bitmap)
    {
      return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
          IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
  }
}
