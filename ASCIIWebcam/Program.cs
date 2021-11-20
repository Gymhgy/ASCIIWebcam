using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

using SkiaSharp;

namespace ASCIIWebcam {
    class Program {

        const string map = "@#%xo;:,. ";

        static void Main(string[] args) {
            int fontSize = 6;
            SKFont font = new SKFont(SKTypeface.FromFamilyName("Consolas"), fontSize);
            SKPaint paint = new SKPaint(font);

            paint.TextAlign = SKTextAlign.Center;
            paint.Color = SKColor.Parse("#000000");
            var metrics = paint.FontMetrics;
            int width = fontSize / 2;
            int height = fontSize;

            int leftRightSize = width / 3;
            int topBottomSize = height / 3;

            ComputeIntensityMap(paint, leftRightSize, topBottomSize);
            VideoCapture capture = new VideoCapture();
            while (true) {
                Console.SetCursorPosition(0, 0);
                using (Mat mat = capture.QueryFrame()) {
                    Image<Gray, byte> img = mat.ToImage<Gray, byte>();

                    byte[,,] data = img.Data;
                    int imgHeight = img.Height, imgWidth = img.Width;
                    int area = width * height;
                    for (int i = 0; i < imgHeight; i += height) {
                        int rectHeight = i + height > imgHeight ? imgHeight - i : height;

                        StringBuilder line = new StringBuilder();
                        for (int j = 0; j < imgWidth; j += width) {
                            int rectWidth = j + width > imgWidth ? imgWidth - j : width;

                            double rightIntensity = 0, leftIntensity = 0, topIntensity = 0, bottomIntensity = 0, centerIntensity = 0;
                            for (int row = i; row < i + rectHeight; row++) {
                                for (int col = j; col < j + rectWidth; col++) {
                                    bool center = true;
                                    if (row < topBottomSize) { center = false; topIntensity += data[row, col, 0]; }
                                    if (row >= height - topBottomSize) { center = false; bottomIntensity += data[row, col, 0]; }

                                    if (col < leftRightSize) { center = false; leftIntensity += data[row, col, 0]; }
                                    if (col >= width - leftRightSize) { center = false; rightIntensity += data[row, col, 0]; }

                                    if (center) centerIntensity += data[row, col, 0];
                                }
                            }
                            rightIntensity /= area;
                            leftIntensity /= area;
                            topIntensity /= area;
                            bottomIntensity /= area;
                            centerIntensity /= area;
                            var intensity = new CharIntensity((char)1, rightIntensity, leftIntensity, topIntensity, bottomIntensity, centerIntensity);
                            char c = intensities.OrderByDescending(charIntensity => charIntensity.DistanceMetric(intensity)).First().Char;
                            line.Append(c.ToString());
                        }
                        Console.WriteLine(line);
                    }
                }
            }
        }

        static List<CharIntensity> intensities = new List<CharIntensity>();

        static void ComputeIntensityMap(SKPaint paint, int leftRightSize, int topBottomSize) {
            var metrics = paint.FontMetrics;
            var descent = metrics.Descent;
            int fontSize = (int)paint.ToFont().Size;
            int width = fontSize / 2;
            int height = fontSize;

            Console.WriteLine(width);
            Console.WriteLine(metrics.XHeight);
            Console.WriteLine(paint.MeasureText("!"));

            SKImageInfo imageInfo = new SKImageInfo(width, height, SKColorType.Gray8);

            using (SKBitmap bitmap = new SKBitmap(imageInfo)) {
                SKCanvas canvas = new SKCanvas(bitmap);
                for (char i = ' '; i < 127; i++) {
                    canvas.Clear(SKColors.White);
                    canvas.DrawText(i.ToString(), width / 2, height-descent, paint);

                    double rightIntensity = 0, leftIntensity = 0, topIntensity = 0, bottomIntensity = 0, centerIntensity = 0;
                    int area = width * height;
                    unsafe {
                        byte* ptr = (byte*)bitmap.GetPixels().ToPointer();
                        for (int row = 0; row < width; row++) {
                            for(int col = 0; col < height; col++) {
                                bool center = true;
                                if (row < topBottomSize) { center = false; topIntensity += *ptr; }
                                if (row >= height - topBottomSize) { center = false; bottomIntensity += *ptr; }

                                if (col < leftRightSize) { center = false; leftIntensity += *ptr; }
                                if (col >= width - leftRightSize) { center = false; rightIntensity += *ptr; }

                                if (center) centerIntensity += *ptr;

                                ptr++;
                            }
                        }
                    }
                    rightIntensity /= area;
                    leftIntensity /= area;
                    topIntensity /= area;
                    bottomIntensity /= area;
                    centerIntensity /= area;
                    intensities.Add(new CharIntensity(i, rightIntensity, leftIntensity, topIntensity, bottomIntensity, centerIntensity));
                }
            }
        }
    }
}
