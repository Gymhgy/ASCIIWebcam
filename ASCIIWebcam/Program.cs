using System;
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
            int cols = 160;
            if (args.Length > 0) {
                int.TryParse(args[0], out cols);
            }

            double ratio = 0.43;

            int tileWidth=0, tileHeight=0, rows=0;
            bool init = false;
            VideoCapture capture = new VideoCapture();
            while (true) {
                Console.SetCursorPosition(0, 0);
                using (Mat mat = capture.QueryFrame()) {
                    Image<Gray, byte> img = mat.ToImage<Gray, byte>();

                    byte[,,] data = img.Data;
                    if (!init) {
                        tileWidth = img.Cols / cols;
                        tileHeight = (int)(tileWidth / ratio);
                        rows = img.Height / tileHeight;
                        ComputeIntensityMap();
                    }
                    for (int i = 0; i < rows; i++) {
                        int y = i * tileHeight, height = i != rows - 1 ? tileHeight : img.Rows - y;

                        StringBuilder line = new StringBuilder();
                        for (int j = 0; j < cols; j++) {
                            int x = j * tileWidth, width = j != cols - 1 ? tileWidth : img.Cols - x;

                            Image<Gray, byte> tile = img.Copy(new Rectangle(x, y, width, height));

                            double mean = CvInvoke.Mean(tile).V0;
                            line.Append(map[(int)((mean / 255) * (map.Length - 1))]);
                        }
                        Console.WriteLine(line);
                    }
                }
            }

        }

        static void ComputeIntensityMap() {
            float fontSize = 18f;
            SKFont font = new SKFont(SKTypeface.FromFamilyName("Consolas"), fontSize);
            SKPaint paint = new SKPaint(font);
            
            paint.TextAlign = SKTextAlign.Center;
            paint.Color = SKColor.Parse("#000000");
            var metrics = paint.FontMetrics;
            var descent = metrics.Descent;
            int width = (int)Math.Ceiling(paint.MeasureText("!")) + 1;
            int height = (int)(fontSize * 4 / 3);
            Console.WriteLine(width);
            SKImageInfo imageInfo = new SKImageInfo(width, height);
            for (char i = ' '; i < 127; i++) {
                using (SKSurface surface = SKSurface.Create(imageInfo)) {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);
                    canvas.DrawText(i.ToString(), width / 2, height-descent, paint);
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
                    using (var stream = File.OpenWrite((int)i+"_abc.png")) {
                        // save the data to a stream
                        data.SaveTo(stream);
                    }
                }
            }
        }
    }
}
