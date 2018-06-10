using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NnCore
{
    public class DigitImage
    {
        // an MNIST image of a '0' thru '9' digit
        public int width; // 28
        public int height; // 28
        public byte[][] pixels; // 0(white) - 255(black)
        public byte label; // '0' - '9'

        public DigitImage(int width, int height, IEnumerable<double> flattened, byte label) :
            this(width, height, PixelArrayFromFlattenedDoubleArray(flattened, width, height), label)
        {

        }

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
            int width = image.Width;
            int height = image.Height;


            byte[][] pixels = new byte[height][];
            for (var i = 0; i < height; ++i)
            {
                pixels[i] = new byte[width];
            }

            // create a C# Bitmap suitable for display in a PictureBox control

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var oc = image.GetPixel(j, i);
                    var grayScale = (byte)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    pixels[i][j] = (byte)(255 - grayScale);
                }
            }
            return new DigitImage(width, height, pixels, label);
        }

        public static byte[][] PixelArrayFromFlattenedDoubleArray(IEnumerable<double> array, int width, int height)
        {
            byte[][] pixels = new byte[height][];
            for (var i = 0; i < height; ++i)
            {
                pixels[i] = new byte[width];
            }
            var ar = new List<double>(array);
            for (int i = 0; i < height; ++i)
            {
                //for (int j = 0; j < width; ++j)
                {
                    pixels[i] = ar.Take(width).Select(d => ConvertDoubleToGrayScaleByte(d)).ToArray();
                    ar.RemoveRange(0, width);
                }
            }
            return pixels;
        }

        public static double ConvertGrayScaleByteToDouble(byte byt)
        {
            return ((byt) / 255.0) /** 0.99 + 0.01*/;
        }
        public static byte ConvertDoubleToGrayScaleByte(double dbl)
        {
            return (byte)(dbl * 255);
        }
    }
}