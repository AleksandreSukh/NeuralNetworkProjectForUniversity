using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NnCore;

namespace NeuralNetworkTSU
{
    public class NnTrainer
    {
        private const int imageWidth = 28;
        private const int imageHeight = 28;
        private static readonly int inputLength = imageWidth * imageHeight;
        private string pixelFile = @"..\..\..\Data\train-images.idx3-ubyte";
        private string labelFile = @"..\..\..\Data\train-labels.idx1-ubyte";
        private NeuralNetwork network;

        private string jsonFilePath = "network.json";
        public int DetectNumberInImage(Bitmap inputImage)
        {
            var di = DigitImage.FromBitmap(inputImage, 1);

            var response = network.Query(di.pixels.SelectMany(su => su.Select(DigitImage.ConvertGrayScaleByteToDouble)).ToArray());

            var max = response.Max(x => x);
            var f = response.ToList().IndexOf(max);
            return f;
        }

        public void LoadData(Action<int> progressUpdater)
        {
            if (File.Exists(jsonFilePath))
            {
                var deserialized = JsonConvert.DeserializeObject<NeuralNetwork>(File.ReadAllText(jsonFilePath));
                network = deserialized;
                progressUpdater.Invoke(100);
                return;
            }
            network = new NeuralNetwork(inputLength, 100, 10, 0.3);



            var database = MnistDataLoader.LoadData(pixelFile, labelFile);
            var toDoubles = database.Select(i => i.pixels.SelectMany(s => s)
                .Select(
                    byt => DigitImage.ConvertGrayScaleByteToDouble(byt))).ToArray();
            //Test2(toDoubles, database);


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
            var networkJson = JsonConvert.SerializeObject(network);
            File.WriteAllText(jsonFilePath, networkJson);
        }

        private static void Test2(IEnumerable<double>[] toDoubles, DigitImage[] database)
        {
            var back = toDoubles.Select(
                ar => new DigitImage(imageWidth, imageHeight, ar, 1)
            ).ToArray();

            for (int i = 0; i < database.Length; i++)
            {
                var fd = database.ElementAt(i);
                var fb = back.ElementAt(i);
                var ser1 = JsonConvert.SerializeObject(fd.pixels);
                var ser2 = JsonConvert.SerializeObject(fb.pixels);
                if (ser1 != ser2)
                {
                    throw new Exception(
                    );
                }
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