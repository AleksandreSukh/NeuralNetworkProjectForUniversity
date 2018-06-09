using System;
using System.Windows.Forms;

// WinForm program for MSDN Magazine article "Image Recognition with the MNIST Data Set"
// Save as MnistViewer.cs
// You must get, download, and unzip the two MNIST data files below
// To compile, create a new VS project, then add this .cs file to the project
// OR use the C# compiler directly: >csc.exe /target:winexe MnistViewer.cs

namespace MnistViewer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    // class Form1


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
    }

} // ns
