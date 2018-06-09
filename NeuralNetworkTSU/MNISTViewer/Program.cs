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
} // ns
