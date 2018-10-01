using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork.NetworkModels
{
    using Helpers;

    /// <summary>   A network. </summary>
    public class Network
	{
		#region -- Properties --

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the learning rate. </summary>
        ///
        /// <value> The learning rate. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public double LearningRate { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the momentum. </summary>
        ///
        /// <value> The momentum. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public double Momentum { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the input layer. </summary>
        ///
        /// <value> The input layer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<Neuron> InputLayer { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the hidden layers. </summary>
        ///
        /// <value> The hidden layers. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<List<Neuron>> HiddenLayers { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the output layer. </summary>
        ///
        /// <value> The output layer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<Neuron> OutputLayer { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the mirror layer. </summary>
        ///
        /// <value> The mirror layer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<Neuron> MirrorLayer {get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the canonical layer. </summary>
        ///
        /// <value> The canonical layer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<Neuron> CanonicalLayer { get; set; }
		#endregion

		#region -- Globals --
        /// <summary>   The random. </summary>
		private static readonly FastRandom Random = new FastRandom();
		#endregion

		#region -- Constructor --

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the NeuralNetwork.NetworkModels.Network class.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public Network()
		{
			LearningRate = 0;
			Momentum = 0;
			InputLayer = new List<Neuron>();
			HiddenLayers = new List<List<Neuron>>();
			OutputLayer = new List<Neuron>();
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the NeuralNetwork.NetworkModels.Network class.
        /// </summary>
        ///
        /// <param name="inputSize">    Size of the input. </param>
        /// <param name="hiddenSizes">  List of sizes of the hiddens. </param>
        /// <param name="outputSize">   Size of the output. </param>
        /// <param name="learnRate">    (Optional) The learn rate. </param>
        /// <param name="momentum">     (Optional) The momentum. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public Network(int inputSize, int[] hiddenSizes, int outputSize, double? learnRate = null, double? momentum = null)
		{
			LearningRate = learnRate ?? .434;
			Momentum = momentum ?? .912;
			InputLayer = new List<Neuron>();
			HiddenLayers = new List<List<Neuron>>();
			OutputLayer = new List<Neuron>();

			for (var i = 0; i < inputSize; i++)
				InputLayer.Add(new Neuron());

			var firstHiddenLayer = new List<Neuron>();
			for (var i = 0; i < hiddenSizes[0]; i++)
				firstHiddenLayer.Add(new Neuron(InputLayer));

			HiddenLayers.Add(firstHiddenLayer);

			for (var i = 1; i < hiddenSizes.Length; i++)
			{
				var hiddenLayer = new List<Neuron>();
				for (var j = 0; j < hiddenSizes[i]; j++)
					hiddenLayer.Add(new Neuron(HiddenLayers[i - 1]));
				HiddenLayers.Add(hiddenLayer);
			}

			for (var i = 0; i < outputSize; i++)
				OutputLayer.Add(new Neuron(HiddenLayers.Last()));
		}
		#endregion

		#region -- Training --

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Trains. </summary>
        ///
        /// <param name="dataSets">     Sets the data belongs to. </param>
        /// <param name="numEpochs">    Number of epochs. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public void Train(List<NNDataSet> dataSets, int numEpochs)
		{
			for (var i = 0; i < numEpochs; i++)
			{
				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);
				}
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Trains. </summary>
        ///
        /// <param name="dataSets">     Sets the data belongs to. </param>
        /// <param name="minimumError"> The minimum error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public void Train(List<NNDataSet> dataSets, double minimumError)
		{
			var error = 1.0;
			var numEpochs = 0;

			while (error > minimumError && numEpochs < int.MaxValue)
			{
				var errors = new List<double>();
				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);
					errors.Add(CalculateError(dataSet.Targets));
				}
				error = errors.Average();
				numEpochs++;
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Forward propagate. </summary>
        ///
        /// <param name="inputs">   A variable-length parameters list containing inputs. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ForwardPropagate(params double[] inputs)
		{
			var i = 0;
			InputLayer?.ForEach(a => a.Value = inputs[i++]);
			HiddenLayers?.ForEach(a => a.ForEach(b => b.CalculateValue()));
			OutputLayer?.ForEach(a => a.CalculateValue());
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Back propagate. </summary>
        ///
        /// <param name="targets">  A variable-length parameters list containing targets. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		private void BackPropagate(params double[] targets)
		{
			var i = 0;
			OutputLayer?.ForEach(a => a.CalculateGradient(targets[i++]));
			HiddenLayers?.Reverse();
			HiddenLayers?.ForEach(a => a.ForEach(b => b.CalculateGradient()));
			HiddenLayers?.ForEach(a => a.ForEach(b => b.UpdateWeights(LearningRate, Momentum)));
			HiddenLayers?.Reverse();
			OutputLayer?.ForEach(a => a.UpdateWeights(LearningRate, Momentum));
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Computes the given inputs. </summary>
        ///
        /// <param name="inputs">   A variable-length parameters list containing inputs. </param>
        ///
        /// <returns>   A double[]. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public double[] Compute(params double[] inputs)
		{
			ForwardPropagate(inputs);
			return OutputLayer.Select(a => a.Value).ToArray();
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Calculates the error. </summary>
        ///
        /// <param name="targets">  A variable-length parameters list containing targets. </param>
        ///
        /// <returns>   The calculated error. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		private double CalculateError(params double[] targets)
		{
			var i = 0;
			return OutputLayer.Sum(a => Math.Abs(a.CalculateError(targets[i++])));
		}
		#endregion

		#region -- Helpers --

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the random. </summary>
        ///
        /// <returns>   The random. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static double GetRandom()
		{
			return 2 * Random.NextDouble() - 1;
		}
		#endregion
	}

	#region -- Enum --
    /// <summary>   Values that represent training types. </summary>
	public enum TrainingType
	{
        /// <summary>   An enum constant representing the epoch option. </summary>
		Epoch,

        /// <summary>   An enum constant representing the minimum error option. </summary>
		MinimumError
	}
	#endregion
}