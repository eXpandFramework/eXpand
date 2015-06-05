using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest{
    public class ImageHelper{
        static readonly IntPtr _desktopHandle = User32.GetDesktopWindow();



        private static User32.RECT GetWindowRect(IntPtr handle){
            var windowRect = new User32.RECT();
            User32.GetWindowRect_Wrapped(handle, ref windowRect);
            return windowRect;
        }

        [SecurityCritical]
        public static void SetWindowSize(IntPtr hWnd, int width, int heigth){
            User32.SetWindowPos_Wrapped(hWnd, IntPtr.Zero, 0, 0, width, heigth, 0);
        }


        private static class NativeHelper{
            public static void ThrowExceptionForHR(){
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error(), new IntPtr(0));
                EasyTestTracer.Tracer.LogText("Marshal.ThrowExceptionForHR is finished without exception.");
                throw new Exception("Marshal.ThrowExceptionForHR is finished without exception.");
            }
        }

        public class GDI32{
            public const int SRCCOPY = 0x00CC0020; 

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight,
                IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll", SetLastError = true)]
            private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void BitBlt_Wrapped(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight,
                IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop){
                EasyTestTracer.Tracer.LogVerboseText(">BitBlt_Wrapped");
                if (!BitBlt(hObject, nXDest, nYDest, nWidth, nHeight, hObjectSource, nXSrc, nYSrc, dwRop)){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<BitBlt_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static IntPtr CreateCompatibleBitmap_Wrapped(IntPtr hDC, int nWidth, int nHeight){
                EasyTestTracer.Tracer.LogVerboseText(">CreateCompatibleBitmap_Wrapped");
                var hBitmap = CreateCompatibleBitmap(hDC, nWidth, nHeight);
                if (hBitmap == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<CreateCompatibleBitmap_Wrapped");
                return hBitmap;
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static IntPtr CreateCompatibleDC_Wrapped(IntPtr hDC){
                EasyTestTracer.Tracer.LogVerboseText(">CreateCompatibleDC_Wrapped");
                var hdcDest = CreateCompatibleDC(hDC);
                if (hdcDest == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<CreateCompatibleDC_Wrapped");
                return hdcDest;
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void DeleteDC_Wrapped(IntPtr hDC){
                EasyTestTracer.Tracer.LogVerboseText(">DeleteDC_Wrapped");
                if (!DeleteDC(hDC)){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<DeleteDC_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void DeleteObject_Wrapped(IntPtr hObject){
                EasyTestTracer.Tracer.LogVerboseText(">DeleteObject_Wrapped");
                if (!DeleteObject(hObject)){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<DeleteObject_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static IntPtr SelectObject_Wrapped(IntPtr hDC, IntPtr hObject){
                EasyTestTracer.Tracer.LogVerboseText(">SelectObject_Wrapped");
                var hOld = SelectObject(hDC, hObject);
                if (hOld == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<SelectObject_Wrapped");
                return hOld;
            }
        }

        public class User32{
            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            private static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            private static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            private static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            //http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
                uint uFlags);

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static IntPtr GetWindowDC_Wrapped(IntPtr hWnd){
                EasyTestTracer.Tracer.LogVerboseText(">GetWindowDC_Wrapped");
                var hdcSrc = GetWindowDC(hWnd);
                if (hdcSrc == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<GetWindowDC_Wrapped");
                return hdcSrc;
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void ReleaseDC_Wrapped(IntPtr hWnd, IntPtr hDC){
                EasyTestTracer.Tracer.LogVerboseText(">ReleaseDC_Wrapped");
                if (ReleaseDC(hWnd, hDC) == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<ReleaseDC_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void GetWindowRect_Wrapped(IntPtr hWnd, ref RECT rect){
                EasyTestTracer.Tracer.LogVerboseText(">GetWindowRect_Wrapped");
                if (GetWindowRect(hWnd, ref rect) == IntPtr.Zero){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<GetWindowRect_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void SetForegroundWindow_Wrapped(IntPtr hWnd){
                EasyTestTracer.Tracer.LogVerboseText(">SetForegroundWindow_Wrapped");
                if (!SetForegroundWindow(hWnd)){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<SetForegroundWindow_Wrapped");
            }

            [SecurityCritical]
            [MethodImpl(MethodImplOptions.NoInlining)] 
            public static void SetWindowPos_Wrapped(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
                uint uFlags){
                EasyTestTracer.Tracer.LogVerboseText(">SetWindowPos_Wrapped");
                if (!SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags)){
                    NativeHelper.ThrowExceptionForHR();
                }
                EasyTestTracer.Tracer.LogVerboseText("<SetWindowPos_Wrapped");
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT{
                public readonly int left;
                public readonly int top;
                public readonly int right;
                public readonly int bottom;
            }
        }
    }

    public class ImageComparer{
        private readonly int _limit;
        private readonly Color _markColor;

        public ImageComparer(int limit, string markColor){
            markColor = "ff" + markColor;
            var value = int.Parse(markColor, NumberStyles.HexNumber);
            _markColor = Color.FromArgb(value);
            _limit = limit;
        }

        public Bitmap GetImagesDiffs(Image img1, Image img2, Image maskImage, out bool areEqual){
            areEqual = true;
            const int absoluteDiffs = 255;
            var bitmap1 = new Bitmap(img1);
            var bitmap2 = new Bitmap(img2);
            var maskBitmap = maskImage != null ? new Bitmap(maskImage) : null;
            var result = new Bitmap(Math.Max(img1.Width, img2.Width), Math.Max(img1.Height, img2.Height),
                PixelFormat.Format32bppRgb);
            if (img1.Width != img2.Width || img1.Height != img2.Height){
                areEqual = false;
            }
            var compareWidth = Math.Min(img1.Width, img2.Width);
            var compareHeight = Math.Min(img1.Height, img2.Height);
            for (var i = 0; i < compareWidth; i++){
                for (var j = 0; j < compareHeight; j++){
                    var isCheckPixel = maskBitmap == null || maskBitmap.GetPixel(i, j).GetBrightness() > 0.5;
                    if (!isCheckPixel){
                        result.SetPixel(i, j, Color.Black);
                    }
                    else{
                        var dR = bitmap1.GetPixel(i, j).R - bitmap2.GetPixel(i, j).R;
                        var dG = bitmap1.GetPixel(i, j).G - bitmap2.GetPixel(i, j).G;
                        var dB = bitmap1.GetPixel(i, j).B - bitmap2.GetPixel(i, j).B;
                        var delta = (int) Math.Sqrt(dR*dR + dG*dG + dB*dB)*100/absoluteDiffs;
                        if (delta <= _limit){
                            result.SetPixel(i, j, Color.Black);
                        }
                        else{
                            result.SetPixel(i, j, Color.White);
                            areEqual = false;
                        }
                    }
                }
            }
            bitmap1.Dispose();
            bitmap2.Dispose();
            return result;
        }

        public Image AddDiffsToImage(Image img1, Bitmap bmp2){
            var bmp1 = new Bitmap(img1);
            var width = Math.Min(img1.Width, bmp2.Width);
            var height = Math.Min(img1.Height, bmp2.Height);
            for (var i = 0; i < width; i++){
                for (var j = 0; j < height; j++){
                    if (bmp2.GetPixel(i, j).R > 0){
                        bmp1.SetPixel(i, j, _markColor);
                    }
                }
            }
            return bmp1;
        }
    }
}