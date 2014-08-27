using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using AForge.Imaging.Filters;

namespace Xpand.Utils.Helpers{
    public static class ImageExtensions{
        private static readonly Font _defaultFont = new Font("Arial", 8);
        private static readonly Brush[] _brushes = new Brush[256];

        static ImageExtensions(){
            for (int i = 0; i < 256; i++){
                _brushes[i] = new SolidBrush(Color.FromArgb(255, i, i/3, i/2));
            }
        }

        public static Image CreateImage(this Image srcImage, int maxWidth, int maxHeight) {
            if (srcImage == null) return null;
            Size size = GetPhotoSize(srcImage, maxWidth, maxHeight);
            Image ret = new Bitmap(size.Width, size.Height);
            using (Graphics gr = Graphics.FromImage(ret)) {
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.DrawImage(srcImage, new Rectangle(0, 0, size.Width, size.Height));
            }
            return ret;
        }

        private static Size GetPhotoSize(Image image, int maxWidth, int maxHeight) {
            int width = Math.Min(maxWidth, image.Width);
            int height = width * image.Height / image.Width;
            if (height > maxHeight) {
                height = maxHeight;
                width = height * image.Width / image.Height;
            }
            return new Size(width, height);
        }

        public static Bitmap ToNonIndexedImage(this Image image) {
            var newBmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(newBmp)) {
                gfx.DrawImage(image, 0, 0);
            }
            return newBmp;
        }

        public static Image ResizeRectangle(this Image image, int width, int height, double brightness = 0.5){
            var bitmap = image;
            var rectangle = bitmap.CalculateRectangle(brightness);
            var indexed = bitmap.PixelFormat == PixelFormat.Format8bppIndexed;
            if (indexed)
                bitmap = bitmap.ToNonIndexedImage();
            using (var graphics = Graphics.FromImage(bitmap)) {
                using (var solidBlackBrush = new SolidBrush(Color.Black))
                    graphics.FillRectangle(solidBlackBrush, rectangle);
                using (var solidWhiteBrush = new SolidBrush(Color.White))
                    graphics.FillRectangle(solidWhiteBrush, new Rectangle(rectangle.X, rectangle.Y, width, height));
            }
            return indexed ? bitmap.ToBlackAndWhite() : bitmap;
        }

        public static Rectangle CalculateRectangle(this Image image,double brightness=0.5){
            var bitmap = ((Bitmap) image);
            var topLeft = CalcTopLeft(image, brightness, bitmap);
            var bottomRight = CalcBottomRight(image, brightness, bitmap);
            return new Rectangle(topLeft, new Size(bottomRight.X-topLeft.X,bottomRight.Y-topLeft.Y));
        }

        private static Point CalcBottomRight(Image image, double brightness, Bitmap bitmap){
            for (int x = image.Width - 1; x > -1; x--){
                for (int y = image.Height - 1; y > -1; y--){
                    if (bitmap.GetPixel(x, y).GetBrightness() > brightness) {
                        return new Point(x+1, y+1);
                    }
                }
            }
            return Point.Empty;
        }

        private static Point CalcTopLeft(Image image, double brightness, Bitmap bitmap){
            for (var x = 0; x < image.Width; x++){
                for (var y = 0; y < image.Height; y++){
                    if (bitmap.GetPixel(x, y).GetBrightness() > brightness) {
                        return new Point(x,y);
                    }
                }
            }
            return Point.Empty;
        }

        public static Image Crop(this Image image,Rectangle cropRectangle) {
            var target = new Bitmap(cropRectangle.Width, cropRectangle.Height);
            using (var graphics = Graphics.FromImage(target)){
                graphics.DrawImage(image, new Rectangle(0, 0, target.Width, target.Height),
                    cropRectangle,GraphicsUnit.Pixel);
            }
            return target;
        }

        public static byte[,] Differences(this Image image1, Image image2, Image mask, double brightness = 0.5) {
            if (image1.Height>image2.Height&&image1.Width>image2.Width)
                return DifferenceCore(image1, image2, mask, brightness);
            if (image2.Height>image1.Height&&image2.Width>image1.Width)
                return DifferenceCore(image2, image1, mask, brightness);
            if (image1.Height > image2.Height && image1.Width == image2.Width)
                return DifferenceCore(image1, image2, mask, brightness);
            if (image2.Height > image1.Height && image2.Width == image1.Width)
                return DifferenceCore(image2, image1, mask, brightness);
            if (image1.Height > image2.Height)
                return DifferenceCore(image1, image2, mask, brightness);
            if (image2.Height > image1.Height)
                return DifferenceCore(image2, image1, mask, brightness);
            return DifferenceCore(image1, image2, mask, brightness);
        }

        private static byte[,] DifferenceCore(Image image1, Image image2, Image mask, double brightness) {
            var rectangle1 = mask.Resize(image1.Width, image1.Height).CalculateRectangle(brightness);
            Rectangle rectangle2=rectangle1;
            if (image1.Width!=image2.Width||image1.Height!=image2.Height)
                rectangle2 = mask.Resize(image2.Width, image2.Height).CalculateRectangle(brightness);
            return image1.Crop(rectangle1).GetDifferences(image2.Crop(rectangle2));
        }

        public static float Difference(this Image image1, Image image2, byte threshold = 3){
            byte[,] differences = image1.GetDifferences(image2);
            return Percentage( differences,threshold);
        }

        public static float Percentage(this byte[,] differences,byte threshold) {
            int diffPixels = 0;
            for (int i = 0; i < differences.GetLength(0); i++)
                for (int i1 = 0; i1 < differences.GetLength(1); i1++){
                    byte b = differences[i, i1];
                    if (b > threshold){
                        diffPixels++;
                    }
                }
            return diffPixels*100/256f;
        }

        public static Bitmap DiffImage(this byte[,] differences, bool adjustColorToMaxDiff = false, bool absoluteText = false) {
            const int cellsize = 16;
            var bmp = new Bitmap(16 * cellsize + 1, 16 * cellsize + 1);
            var graphics = Graphics.FromImage(bmp);
            graphics.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
            var maxDifference = GetMaxDifference(adjustColorToMaxDiff, differences);
            DiffImage(absoluteText, differences, maxDifference, graphics, cellsize);
            return bmp;
        }

        public static Bitmap DiffImage(this Image image1, Image image2,bool adjustColorToMaxDiff = false, bool absoluteText = false){
            var differences = image1.GetDifferences(image2);
            return differences.DiffImage(adjustColorToMaxDiff, absoluteText);
        }

        private static void DiffImage(bool absoluteText, byte[,] differences, byte maxDifference, Graphics g, int cellsize){
            for (int y = 0; y < differences.GetLength(1); y++){
                for (int x = 0; x < differences.GetLength(0); x++){
                    var cellText = GetCellText(absoluteText, differences, x, y);
                    float percentageDifference = (float) differences[x, y]/maxDifference;
                    var colorIndex = (int) (255*percentageDifference);
                    DiffImage(g, cellsize, colorIndex, x, y, cellText);
                }
            }
        }

        private static void DiffImage(Graphics g, int cellsize, int colorIndex, int x, int y, string cellText){
            g.FillRectangle(_brushes[colorIndex], x*cellsize, y*cellsize, cellsize, cellsize);
            g.DrawRectangle(Pens.Blue, x*cellsize, y*cellsize, cellsize, cellsize);
            SizeF size = g.MeasureString(cellText, _defaultFont);
            g.DrawString(cellText, _defaultFont, Brushes.Black, x*cellsize + cellsize/2 - size.Width/2 + 1,
                y*cellsize + cellsize/2 - size.Height/2 + 1);
            g.DrawString(cellText, _defaultFont, Brushes.White, x*cellsize + cellsize/2 - size.Width/2,
                y*cellsize + cellsize/2 - size.Height/2);
        }

        private static string GetCellText(bool absoluteText, byte[,] differences, int x, int y){
            byte cellValue = differences[x, y];
            return absoluteText
                ? cellValue.ToString(CultureInfo.InvariantCulture)
                : string.Format("{0}%", (int) cellValue);
        }

        private static byte GetMaxDifference(bool adjustColorToMaxDiff, byte[,] differences){
            byte maxDifference = 255;
            if (adjustColorToMaxDiff){
                maxDifference = 0;
                foreach (byte b in differences){
                    if (b > maxDifference){
                        maxDifference = b;
                    }
                }
                if (maxDifference == 0){
                    maxDifference = 1;
                }
            }
            return maxDifference;
        }

        public static byte[,] GetDifferences(this Image img1, Image img2){
            var thisOne = (Bitmap) img1.Resize(16, 16).ToGrayScale();
            var theOtherOne = (Bitmap) img2.Resize(16, 16).ToGrayScale();
            var differences = new byte[16, 16];
            for (int y = 0; y < 16; y++){
                for (int x = 0; x < 16; x++){
                    differences[x, y] = (byte) Math.Abs(thisOne.GetPixel(x, y).R - theOtherOne.GetPixel(x, y).R);
                }
            }
            return differences;
        }

        public static Image ToBlackAndWhite(this Image original){
            var filtersSequence = new FiltersSequence { Grayscale.CommonAlgorithms.BT709,new OtsuThreshold() };
            return filtersSequence.Apply((Bitmap)original);
        }

        public static Image ToGrayScale(this Image original){
            var filtersSequence = new FiltersSequence{Grayscale.CommonAlgorithms.BT709};
            return filtersSequence.Apply((Bitmap) original);
        }

        public static Image Resize(this Image originalImage, int newWidth, int newHeight) {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion)) {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return smallVersion;
        }

        public static float GetPercentageDifference(string image1Path, string image2Path, byte threshold = 3){
            if (CheckFile(image1Path) && CheckFile(image2Path)){
                Image img1 = Image.FromFile(image1Path);
                Image img2 = Image.FromFile(image2Path);

                return img1.Difference(img2, threshold);
            }
            return -1;
        }

        private static bool CheckFile(string filePath){
            if (!File.Exists(filePath)){
                throw new FileNotFoundException("File '" + filePath + "' not found!");
            }
            return true;
        }

        public static Rectangle GetZoomDestRectangle(this Image img, Rectangle r){
            float horzRatio = Math.Min((float) r.Width/img.Width, 1);
            float vertRatio = Math.Min((float) r.Height/img.Height, 1);
            float zoomRatio = Math.Min(horzRatio, vertRatio);

            return new Rectangle(
                r.Left + (int) (r.Width - img.Width*zoomRatio)/2,
                r.Top + (int) (r.Height - img.Height*zoomRatio)/2,
                (int) (img.Width*zoomRatio),
                (int) (img.Height*zoomRatio));
        }
    }
}