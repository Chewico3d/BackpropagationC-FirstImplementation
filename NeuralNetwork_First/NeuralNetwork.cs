using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork_First
{
    public class NeuralNetwork
    {

        public const float LearningRate = 0.01f;

        //The Base Random Generator
        public static Random randomGenerator;
        //The number of neurons per layer
        public int[] LayersSizes;

        //Array of arrays el numero es -1
        public float[][] Bias;
        //Values for FrontPropagation
        public float[][] Values;
        //Layer ID, Neuron From, Neuron To
        public float[][][] Weights;
        //Derivatives
        public float[][][] Derivatives;

        public float[][] NeuronDerivativeError;
        public float[] Errors;

        //SimpleInitializer
        public NeuralNetwork(int[] LayersSizes)
        {
            randomGenerator = new Random(DateTime.Now.Millisecond);

            this.LayersSizes = LayersSizes;
            this.Values = new float[LayersSizes.Length][];
            Weights = new float[LayersSizes.Length - 1][][];
            Derivatives = new float[LayersSizes.Length - 1][][];
            Bias = new float[LayersSizes.Length][];

            Errors = new float[LayersSizes[LayersSizes.Length - 1]];

            NeuronDerivativeError = new float[LayersSizes.Length][];

            //Layer sizes - 1 because weights have 1 layer minus
            for (int i = 0; i < LayersSizes.Length - 1; i++)
            {
                Values[i] = new float[LayersSizes[i]];
                Weights[i] = new float[LayersSizes[i]][];
                Derivatives[i] = new float[LayersSizes[i]][];
                Bias[i] = new float[LayersSizes[i]];

                NeuronDerivativeError[i] = new float[LayersSizes[i]];

                for(int y = 0; y < LayersSizes[i]; y++)
                    Bias[i][y] = (float)randomGenerator.NextDouble() * 2 - 1f;//

                for (int y = 0; y < LayersSizes[i]; y++)
                {
                    Weights[i][y] = new float[LayersSizes[i + 1]];
                    Derivatives[i][y] = new float[LayersSizes[i + 1]];

                    for (int z = 0; z < LayersSizes[i + 1]; z++)
                        Weights[i][y][z] = (float)randomGenerator.NextDouble() * 2 - 1f;

                }
            }
            Bias[LayersSizes.Length - 1] = new float [LayersSizes[LayersSizes.Length - 1]];
            Values[LayersSizes.Length - 1] = new float[LayersSizes[LayersSizes.Length - 1]];
            NeuronDerivativeError[LayersSizes.Length - 1] = new float[LayersSizes[LayersSizes.Length - 1]];
        }

        //FronPropagate
        public void FrontPropagation()
        {
            //Repetition For every layer
            for(int x = 0; x < LayersSizes.Length - 1; x++)
            {
                //Repetition for every neuron in front of the layer(x + 1)
                for (int y = 0; y < LayersSizes[x + 1]; y++)
                {
                    //Set the next neuron to 0
                    Values[x + 1][y] = 0;

                    //Repetition for every neuron in the last layer(x)
                    for(int z = 0; z < LayersSizes[x]; z++)
                    {
                        //Last layer * weight
                        Values[x + 1][y] += Values[x][z] * Weights[x][z][y];

                    }
                    //AdBias
                    Values[x + 1][y] += Bias[x + 1][y];

                    //Pas by the sigmoid Funtion
                    Values[x + 1][y] = SigmoidFuntion(Values[x + 1][y]);

                }

            }

        }

        //The Sigmoid Funtion Used S(x)
        private float SigmoidFuntion(float Value) => 1 / (1 + MathF.Pow(2.71f, -Value));
        //private float SigmoidFuntion(float Value) => (Value < 0)? 0 : Value;
        //The derivative of sigmoid Funtion S'(x) = S(x)(1-S(x))
        private float DerivativeSigmoidFuntion(float Value)
        {
            //Sigmoid Passed Value
            float SPV = SigmoidFuntion(Value);
            return SPV * (1 - SPV);
        }
        private float DerivativeSigmoidFuntionFromSigmoid(float Value)
        {
            //Sigmoid Passed Value
            return Value * (1 - Value);
        }
        //private float DerivativeSigmoidFuntionFromSigmoid(float Value) => (Value < 0)? 0 : 1;

        //Cost funtion : C(x) = (y-x)^2
        //The derivative of this is 2(y-x)
        private float Cost(float ExpectedValue, float Value)
        {
            float cost = ExpectedValue - Value;
            return cost * cost;

        }
        private float CostDerivate(float ExpectedValue, float Value) 
        {
            float cost = Value - ExpectedValue;
            return cost * 2;

        }

        //BackPropagate
        public void BackPropagate(float[] ExpectedValues)
        {
            //Console.WriteLine(Bias.Length + " ...");
            //Bias[1][0] = 5;
            //Weights[0][0][0] = -5;
            //Weights[0][1][0] = -5;

            for (int y = 0; y < ExpectedValues.Length; y++)
            {
                Errors[y] = Cost(ExpectedValues[y], Values[LayersSizes.Length - 1][y]);

                NeuronDerivativeError[LayersSizes.Length - 1][y] = CostDerivate(ExpectedValues[y], Values[LayersSizes.Length - 1][y]);
                NeuronDerivativeError[LayersSizes.Length - 1][y] *= DerivativeSigmoidFuntionFromSigmoid(Values[LayersSizes.Length - 1][y]);

                Bias[LayersSizes.Length - 1][y] -= NeuronDerivativeError[LayersSizes.Length - 1][y] * LearningRate;//NeuronDerivativeError[x][y] * LearningRate

            }

            //From the layer size - 2 to the 0
            for(int x = LayersSizes.Length - 2; x >= 0; x--)
            {
                //Calculate Weight Derivatives
                for(int y = 0; y < LayersSizes[x]; y++)
                {
                    NeuronDerivativeError[x][y] = 0;

                    for (int z = 0; z < LayersSizes[x + 1]; z++)
                    {
                        float WeightDerivative = NeuronDerivativeError[x + 1][z];
                        WeightDerivative = WeightDerivative * Values[x][y];

                        Derivatives[x][y][z] = WeightDerivative;
                        Weights[x][y][z] -= WeightDerivative * LearningRate;

                        NeuronDerivativeError[x][y] -= Derivatives[x][y][z];

                    }

                    //Set the neuron error
                    NeuronDerivativeError[x][y] /= LayersSizes[x + 1];
                    NeuronDerivativeError[x][y] *= DerivativeSigmoidFuntionFromSigmoid(Values[x][y]);

                    Bias[x][y] -= NeuronDerivativeError[x][y] * LearningRate;//NeuronDerivativeError[x][y] * LearningRate

                }
                //Calculate New neurons Derivatives

            }

        }

    }
}
