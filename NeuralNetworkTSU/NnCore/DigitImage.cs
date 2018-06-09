using System;
using System.Drawing;

namespace NnCore
{
    public class DigitImage
    {
        // an MNIST image of a '0' thru '9' digit
        public int width; // 28
        public int height; // 28
        public byte[][] pixels; // 0(white) - 255(black)
        public byte label; // '0' - '9'

        public DigitImage(int width, int height, byte[][] pixels, byte label)
        {
            this.width = width; this.height = height;
            this.pixels = new byte[height][];
            for (int i = 0; i < this.pixels.Length; ++i)
                this.pixels[i] = new byte[width];

            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    this.pixels[i][j] = pixels[i][j];

            this.label = label;
        }
        public static Bitmap MakeBitmap(DigitImage dImage, int mag)
        {
            // create a C# Bitmap suitable for display in a PictureBox control
            int width = dImage.width * mag;
            int height = dImage.height * mag;
            Bitmap result = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(result);
            for (int i = 0; i < dImage.height; ++i)
            {
                for (int j = 0; j < dImage.width; ++j)
                {
                    int pixelColor = 255 - dImage.pixels[i][j]; // white background, black digits
                    //int pixelColor = dImage.pixels[i][j]; // black background, white digits
                    Color c = Color.FromArgb(pixelColor, pixelColor, pixelColor); // gray scale
                    //Color c = Color.FromArgb(pixelColor, 0, 0); // red scale
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * mag, i * mag, mag, mag); // fills bitmap via Graphics object
                }
            }
            return result;
        }

        public static DigitImage FromBitmap(Bitmap image, byte label)
        {
            byte[][] pixels = new byte[image.Width][];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new byte[image.Height];
            }

            // create a C# Bitmap suitable for display in a PictureBox control
            int width = image.Width;
            int height = image.Height;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var oc = image.GetPixel(i, j);
                    var grayScale = (byte)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    pixels[i][j] = grayScale;
                }
            }
            return new DigitImage(width, height, pixels, label);
        }
    }
}