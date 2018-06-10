using System;
using System.Linq;

namespace NnCore
{
    [Serializable]
    public class NeuralNetwork
    {
        public NeuralNetwork()
        {
            //For Serializer
        }

        private double _learningRate;
        private Matrix _weightHiddenOutput;
        private Matrix _weightInputHidden;

        public NeuralNetwork(int numberOfInputNodes, int numberOfHiddenNodes, int numberOfOutputNodes, double learningRate)
        {
            LearningRate = learningRate;

            WeightInputHidden = Matrix.Create(numberOfHiddenNodes, numberOfInputNodes);
            WeightHiddenOutput = Matrix.Create(numberOfOutputNodes, numberOfHiddenNodes);

            RandomizeWeights();
        }

        //Public properties to make it serializable
        public Matrix WeightInputHidden
        {
            get { return _weightInputHidden; }
            set { _weightInputHidden = value; }
        }

        public Matrix WeightHiddenOutput
        {
            get { return _weightHiddenOutput; }
            set { _weightHiddenOutput = value; }
        }

        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }
        //!

        private void RandomizeWeights()
        {
            var rnd = new Random();

            //distribute -0.5 to 0.5.
            WeightHiddenOutput.Initialize(() => rnd.NextDouble() - 0.5);
            WeightInputHidden.Initialize(() => rnd.NextDouble() - 0.5);
        }

        public void Train(double[] inputs, double[] targets)
        {
            var inputSignals = ConvertToMatrix(inputs);
            var targetSignals = ConvertToMatrix(targets);

            var hiddenOutputs = Sigmoid(WeightInputHidden * inputSignals);
            var finalOutputs = Sigmoid(WeightHiddenOutput * hiddenOutputs);

            var outputErrors = targetSignals - finalOutputs;

            var hiddenErrors = WeightHiddenOutput.Transpose() * outputErrors;

            WeightHiddenOutput += LearningRate * outputErrors * finalOutputs * (1.0 - finalOutputs) * hiddenOutputs.Transpose();
            WeightInputHidden += LearningRate * hiddenErrors * hiddenOutputs * (1.0 - hiddenOutputs) * inputSignals.Transpose();
        }

        public double[] Query(double[] inputs)
        {
            var inputSignals = ConvertToMatrix(inputs);

            var hiddenOutputs = Sigmoid(WeightInputHidden * inputSignals);
            var finalOutputs = Sigmoid(WeightHiddenOutput * hiddenOutputs);

            return finalOutputs.Value.SelectMany(x => x.Select(y => y)).ToArray();
        }

        private static Matrix ConvertToMatrix(double[] inputList)
        {
            var input = new double[inputList.Length][];

            for (var x = 0; x < input.Length; x++)
            {
                input[x] = new[] { inputList[x] };
            }

            return Matrix.Create(input);
        }

        private Matrix Sigmoid(Matrix matrix)
        {
            var newMatrix = Matrix.Create(matrix.Value.Length, matrix.Value[0].Length);

            for (var x = 0; x < matrix.Value.Length; x++)
            {
                for (var y = 0; y < matrix.Value[x].Length; y++)
                {
                    newMatrix.Value[x][y] = 1 / (1 + Math.Pow(Math.E, -matrix.Value[x][y]));
                }
            }

            return newMatrix;
        }
    }
}