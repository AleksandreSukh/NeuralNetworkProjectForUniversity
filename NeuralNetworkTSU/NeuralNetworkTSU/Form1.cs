using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NnCore;

namespace NeuralNetworkTSU
{
    public partial class Form1 : Form
    {
        //TODO:მერე გავიტანო სადმე 
        static int inputLength = 784; //28 x 28 
        private string pixelFile = @"..\..\..\Data\train-images.idx3-ubyte";
        private string labelFile = @"..\..\..\Data\train-labels.idx1-ubyte";
        List<bool> scoreCard = new List<bool>();





        private void button3_Click(object sender, EventArgs e)
        {
            var inputImage = (Bitmap)pictureBox1.Image;

            var di = DigitImage.FromBitmap(inputImage, 1);

            var response = network.Query(di.pixels.SelectMany(su => su.Select(ConvertGrayScaleByteToDouble)).ToArray());

            var max = response.Max(x => x);
            var f = response.ToList().IndexOf(max);
            lblDetectedNumber.Text = f.ToString();

        }

        Bitmap DrawArea;
        public Form1()
        {
            InitializeComponent();

            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;

        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            LoadData();
        }

        private NeuralNetwork network;//TODO:
        private void LoadData()
        {
            network = new NeuralNetwork(inputLength, 100, 10, 0.3);
            var database = MnistDataLoader.LoadData(pixelFile, labelFile);
            var flattenedArrays = database.Select(i => i.pixels.SelectMany(s => s));
            var toDoubles = flattenedArrays.Select(
                ar => ar.Select(
                    byt => ConvertGrayScaleByteToDouble(byt))
                )
                .ToArray();
            var targets = database.Select(i => Convert.ToInt32(i.label)).ToArray();




            for (var index = 0; index < toDoubles.Length; index++)
            {
                var doubleArray = toDoubles[index];
                var correctAnswer = targets[index];
                var target = Enumerable.Range(0, 10).Select(x => 0.01).ToArray();
                target[correctAnswer] = 0.99;

                network.Train(doubleArray.ToArray(), target);
            }
        }

        private static double ConvertGrayScaleByteToDouble(byte byt)
        {
            return ((byt) / 255.0) * 0.99 + 0.01;
        }

        void ToImage()
        {
            //int nextIndex = int.Parse(textBox4.Text);
            //DigitImage currImage = trainImages[nextIndex];

            //int mag = int.Parse(comboBox1.SelectedItem.ToString()); // magnification
            //Bitmap bitMap = DigitImage.MakeBitmap(currImage, mag);
        }
        private static double[] NormalizeInput(string[] input)
        {
            return input
                .Select(double.Parse)
                .Select(y => (y / 255) * 0.99 + 0.01)
                .ToArray();
        }

        private Point? _Previous = null;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _Previous = e.Location;
            pictureBox1_MouseMove(sender, e);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Previous != null)
            {
                if (pictureBox1.Image == null)
                {
                    Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                    }
                    pictureBox1.Image = bmp;
                }
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.DrawLine(Pens.Black, _Previous.Value, e.Location);
                }
                pictureBox1.Invalidate();
                _Previous = e.Location;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _Previous = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }
    }
}
