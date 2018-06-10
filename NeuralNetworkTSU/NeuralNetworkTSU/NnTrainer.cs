using System;
using System.Drawing;
using System.Linq;
using NnCore;

namespace NeuralNetworkTSU
{
    public class NnTrainer
    {
        //TODO:მერე გავიტანო სადმე 
        static int inputLength = 784; //28 x 28 
        private string pixelFile = @"..\..\..\Data\train-images.idx3-ubyte";
        private string labelFile = @"..\..\..\Data\train-labels.idx1-ubyte";
        private NeuralNetwork network;//TODO:
        public int DetectNumberInImage(Bitmap inputImage)
        {
            var di = DigitImage.FromBitmap(inputImage, 1);

            var response = network.Query(di.pixels.SelectMany(su => su.Select(ConvertGrayScaleByteToDouble)).ToArray());

            var max = response.Max(x => x);
            var f = response.ToList().IndexOf(max);
            return f;
        }

        //public event EventHandler<int> DataLoadingPercentChanged;
        private static double ConvertGrayScaleByteToDouble(byte byt)
        {
            return ((byt) / 255.0) * 0.99 + 0.01;
        }
        public void LoadData(Action<int> progressUpdater)
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



            var percent = 0;

            for (var index = 0; index < toDoubles.Length; index++)
            {
                var doubleArray = toDoubles[index];
                var correctAnswer = targets[index];
                var target = Enumerable.Range(0, 10).Select(x => 0.01).ToArray();
                target[correctAnswer] = 0.99;

                percent = NotifyDataLoadingProgress(index, toDoubles.Length, percent, progressUpdater);

                network.Train(doubleArray.ToArray(), target);
            }
        }

        private int NotifyDataLoadingProgress(int index, int length, int percent, Action<int> progressUpdater)
        {
            var currentPercent = (int)(((index + 1) / Convert.ToDouble(length)) * 100);
            {
                if (currentPercent != percent)
                {
                    progressUpdater.Invoke(currentPercent);
                }

                percent = currentPercent;
            }
            return percent;
        }
    }
}