using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NnCore;

namespace NeuralNetworkTSU
{
    public partial class Form1 : Form
    {
        public delegate void UpdateProgressBarDelegate(int progress);   // similar to UpdateProgressBar method

        public UpdateProgressBarDelegate UpdateProgressBar;

        // your form's methods and properties here, as usual

        private void UpdateProgressBarMethod(int progress)
        {
            progressBar1.Value = progress;
            progressBar1.Refresh();
        }
        private NnTrainer trainer = new NnTrainer();
        private void button3_Click(object sender, EventArgs e)
        {
            var inputImage = GetInputImage();
            var f = trainer.DetectNumberInImage(inputImage);
            lblDetectedNumber.Text = f.ToString();
        }

        private Bitmap GetInputImage()
        {
            var inputImage = (Bitmap) pictureBox1.Image;
            if (inputImage == null) inputImage = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);

            return inputImage;
        }


        Bitmap DrawArea;
        public Form1()
        {
            UpdateProgressBar = new UpdateProgressBarDelegate(UpdateProgressBarMethod);
            InitializeComponent();
            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;

        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            TrainNetwork();
        }

        private void TrainNetwork()
        {
            Task.Run(() =>
            {
                trainer.LoadData(i => this.Invoke(UpdateProgressBar, i));
                MessageBox.Show("Data loading finished");
            });
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

                var pen = new Pen(new SolidBrush(Color.Black), 1.7f);
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.DrawLine(pen, _Previous.Value, e.Location);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            TrainNetwork();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            var mag = DigitImage.MakeBitmap(DigitImage.FromBitmap(new Bitmap(GetInputImage()), 1), 10);
            pictureBox2.Image = mag;


            pictureBox2.Invalidate();
        }
    }
}
