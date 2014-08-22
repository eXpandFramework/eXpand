using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Xpand.Utils.Helpers{
    public static class ImageExtensions{
        private static readonly Font _defaultFont = new Font("Arial", 8);
        private static readonly Brush[] _brushes = new Brush[256];

        private static readonly ColorMatrix _colorMatrix = new ColorMatrix(new[]{
            new[]{.3f, .3f, .3f, 0, 0},
            new[]{.59f, .59f, .59f, 0, 0},
            new[]{.11f, .11f, .11f, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });

        static ImageExtensions(){
            for (int i = 0; i < 256; i++){
                _brushes[i] = new SolidBrush(Color.FromArgb(255, i, i/3, i/2));
            }
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
                        return new Point(x, y);
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

        public static Image ToGrayScale(this Image original){
            var newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);
            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(_colorMatrix);
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();

            return newBitmap;
        }

        public static Image Resize(this Image originalImage, int newWidth, int newHeight){
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion)){
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return smallVersion;
        }

        public static Bitmap GetRgbHistogramBitmap(this Bitmap bmp){
            return new Histogram(bmp).Visualize();
        }

        public static Histogram GetRgbHistogram(this Bitmap bmp){
            return new Histogram(bmp);
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

        public static Image CreateImage(this Image srcImage, int maxWidth, int maxHeight){
            if (srcImage == null) return null;
            Size size = GetPhotoSize(srcImage, maxWidth, maxHeight);
            Image ret = new Bitmap(size.Width, size.Height);
            using (Graphics gr = Graphics.FromImage(ret)){
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.DrawImage(srcImage, new Rectangle(0, 0, size.Width, size.Height));
            }
            return ret;
        }

        private static Size GetPhotoSize(Image image, int maxWidth, int maxHeight){
            int width = Math.Min(maxWidth, image.Width);
            int height = width*image.Height/image.Width;
            if (height > maxHeight){
                height = maxHeight;
                width = height*image.Width/image.Height;
            }
            return new Size(width, height);
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

    public class Histogram{
        private static readonly Pen[] _p = {Pens.Red, Pens.Green, Pens.Blue};

        public Histogram(Bitmap bitmap){
            Bitmap = bitmap;
            Red = new byte[256];
            Green = new byte[256];
            Blue = new byte[256];
            CalculateHistogram();
        }

        public Histogram(string filePath) : this((Bitmap) Image.FromFile(filePath)){
        }
        public byte[] Red { get; private set; }
        public byte[] Green { get; private set; }
        public byte[] Blue { get; private set; }

        public Bitmap Bitmap { get; private set; }


        private void CalculateHistogram(){
            var newBmp = (Bitmap) Bitmap.Resize(16, 16);
            for (int x = 0; x < newBmp.Width; x++){
                for (int y = 0; y < newBmp.Height; y++){
                    var c = newBmp.GetPixel(x, y);
                    Red[c.R]++;
                    Green[c.G]++;
                    Blue[c.B]++;
                }
            }
        }

        public Bitmap Visualize(){
            const int oneColorHeight = 100;
            const int margin = 10;

            float[] maxValues = {Red.Max(), Green.Max(), Blue.Max()};
            byte[][] values = {Red, Green, Blue};


            var histogramBitmap = new Bitmap(276, oneColorHeight*3 + margin*4);
            Graphics g = Graphics.FromImage(histogramBitmap);
            g.FillRectangle(Brushes.White, 0, 0, histogramBitmap.Width, histogramBitmap.Height);
            const int yOffset = margin + oneColorHeight;

            for (int i = 0; i < 256; i++){
                for (int color = 0; color < 3; color++){
                    g.DrawLine(_p[color], margin + i, yOffset*(color + 1), margin + i,
                        yOffset*(color + 1) - (values[color][i]/maxValues[color])*oneColorHeight);
                }
            }

            for (int i = 0; i < 3; i++){
                g.DrawString(_p[i].Color.ToKnownColor() + ", max value: " + maxValues[i], SystemFonts.SmallCaptionFont,
                    Brushes.Silver, margin + 11, yOffset*i + margin + margin + 1);
                g.DrawString(_p[i].Color.ToKnownColor() + ", max value: " + maxValues[i], SystemFonts.SmallCaptionFont,
                    Brushes.Black, margin + 10, yOffset*i + margin + margin);
                g.DrawRectangle(_p[i], margin, yOffset*i + margin, 256, oneColorHeight);
            }
            g.Dispose();

            return histogramBitmap;
        }

        public override string ToString(){
            var sb = new StringBuilder();

            for (int i = 0; i < 256; i++){
                sb.Append(string.Format("RGB {0,3} : ", i) +
                          string.Format("({0,3},{1,3},{2,3})", Red[i], Green[i], Blue[i]));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public float GetVariance(Histogram histogram){
            double diffRed = 0, diffGreen = 0, diffBlue = 0;
            for (int i = 0; i < 256; i++){
                diffRed += Math.Pow(Red[i] - histogram.Red[i], 2);
                diffGreen += Math.Pow(Green[i] - histogram.Green[i], 2);
                diffBlue += Math.Pow(Blue[i] - histogram.Blue[i], 2);
            }

            diffRed /= 256;
            diffGreen /= 256;
            diffBlue /= 256;
            const double maxDiff = 512;
            return (float) (diffRed/maxDiff + diffGreen/maxDiff + diffBlue/maxDiff)/3;
        }
    }
}