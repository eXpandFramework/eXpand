using System.Drawing;
using System.Runtime.InteropServices;
using AForge.Video;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Helpers{
    public class ScreenCaptureStream : AForge.Video.ScreenCaptureStream{
        private bool _drawCursor = true;

        public ScreenCaptureStream(Rectangle region) : base(region){
            NewFrame+=OnNewFrame;
        }

        public ScreenCaptureStream(Rectangle region, int frameInterval) : base(region, frameInterval){
            NewFrame+=OnNewFrame;
        }

        public bool DrawCursor{
            get { return _drawCursor; }
            set { _drawCursor = value; }
        }

        public string FileName { get; set; }

        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs){
            if (DrawCursor){
                using (var g = Graphics.FromImage(eventArgs.Frame)) {
                    Win32Declares.MouseCursor.CURSORINFO pci;
                    pci.cbSize = Marshal.SizeOf(typeof(Win32Declares.MouseCursor.CURSORINFO));
                    if (Win32Declares.MouseCursor.GetCursorInfo(out pci)) {
                        if (pci.flags == Win32Declares.MouseCursor.CURSOR_SHOWING) {
                            Win32Declares.MouseCursor.DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            g.ReleaseHdc();
                        }
                    }
                }
            }
        }
    }
}