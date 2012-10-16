using System;
using System.Drawing;

namespace Xpand.Utils.Helpers {
    public static class ImageExtensions {
        public static Image CreateImage(this Image srcImage, int maxWidth, int maxHeight) {
            if (srcImage == null) return null;
            Size size = GetPhotoSize(srcImage, maxWidth, maxHeight);
            Image ret = new Bitmap(size.Width, size.Height);
            using (Graphics gr = Graphics.FromImage(ret)) {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.DrawImage(srcImage, new Rectangle(0, 0, size.Width, size.Height));
            }
            return ret;
        }
        static Size GetPhotoSize(Image image, int maxWidth, int maxHeight) {
            int width = Math.Min(maxWidth, image.Width),
                height = width * image.Height / image.Width;
            if (height > maxHeight) {
                height = maxHeight;
                width = height * image.Width / image.Height;
            }
            return new Size(width, height);
        }
        public static Rectangle GetZoomDestRectangle(Rectangle r, Image img) {
            float horzRatio = Math.Min((float)r.Width / img.Width, 1);
            float vertRatio = Math.Min((float)r.Height / img.Height, 1);
            float zoomRatio = Math.Min(horzRatio, vertRatio);

            return new Rectangle(
                r.Left + (int)(r.Width - img.Width * zoomRatio) / 2,
                r.Top + (int)(r.Height - img.Height * zoomRatio) / 2,
                (int)(img.Width * zoomRatio),
                (int)(img.Height * zoomRatio));
        }
    }
}