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
            paint.Color = SKColor.Parse("#FFFFFF");

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

                            double intensity = 0;
                            for (int row = i; row < i + rectHeight; row++) {
                                for (int col = j; col < j + rectWidth; col++) {
                                    intensity += 255-data[row, col, 0];
                                }
                            }

                            char c = intensities.OrderBy(charIntensity => Math.Abs(charIntensity.intensity - intensity)).First().c;
                            line.Append(c.ToString());
                        }
                        Console.WriteLine(line);
                    }
                }
            }
        }

        static List<(char c, double intensity)> intensities = new List<(char c, double intensity)>();

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
                    canvas.Clear(SKColors.Black);
                    canvas.DrawText(i.ToString(), 0, height-descent, paint);

                    double intensity = 0;
                    int area = width * height;
                    unsafe {
                        byte* ptr = (byte*)bitmap.GetPixels().ToPointer();
                        for (int pixel = 0; pixel < area; pixel++) {
                            intensity += *ptr++;
                        }
                    }

                    intensities.Add((i, intensity));
                }
            }
        }
    }
}
