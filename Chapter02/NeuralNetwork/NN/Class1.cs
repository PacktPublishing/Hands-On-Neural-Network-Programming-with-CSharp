using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace NeuralNetwork
{
    using System.Drawing;
    using Helpers;
    using NetworkModels;

    /// <summary>   Manager for nns. </summary>
    public class NNManager
    {
        		#region -- Variables --
        /// <summary>   Number of input parameters. </summary>
		private  int _numInputParameters;
        /// <summary>   Number of hidden layers. </summary>
		private  int _numHiddenLayers;
        /// <summary>   The hidden neurons. </summary>
		private  int[] _hiddenNeurons;
        /// <summary>   Number of output parameters. </summary>
		private  int _numOutputParameters;
        /// <summary>   The network. </summary>
		private  Network _network;
        /// <summary>   Sets the data belongs to. </summary>
		private  List<NNDataSet> _dataSets;
		#endregion

		#region -- Main --
        /// <summary>   Main entry-point for this application. </summary>
		[STAThread]
		private  void Main()
		{
		}
		#endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets up the network. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager SetupNetwork()
		{
			_numInputParameters = 2;

            int[] hidden = new int[2];
		    hidden[0] = 3;
		    hidden[1] = 1;
            _numHiddenLayers = 1;
            _hiddenNeurons = hidden;
            _numOutputParameters = 1;
			_network = new Network(_numInputParameters, _hiddenNeurons, _numOutputParameters);
			return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets number input parameters. </summary>
        ///
        /// <param name="num">  Number of. </param>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager SetNumInputParameters(int num)
		{
		    _numInputParameters = num;
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets number neurons in hidden layer. </summary>
        ///
        /// <param name="hiddenNeurons">    The hidden neurons. </param>
        /// <param name="numHiddenLayers">  (Optional) Number of hidden layers. </param>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager SetNumNeuronsInHiddenLayer(int[] hiddenNeurons, int numHiddenLayers = 1)
		{
			_numHiddenLayers = numHiddenLayers;
			_hiddenNeurons = hiddenNeurons;
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets number output parameters. </summary>
        ///
        /// <param name="numOutputs">   (Optional) Number of outputs. </param>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager SetNumOutputParameters(int numOutputs = 1)
		{
			_numOutputParameters = numOutputs;
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets training data from user. </summary>
        ///
        /// <returns>   The training data from user. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  NNManager GetTrainingDataFromUser()
		{
			PrintNewLine();

			var numDataSets = GetInput("\tHow many datasets are you going to enter? ", 1, int.MaxValue);

			var newDatasets = new List<NNDataSet>();
			for (var i = 0; i < numDataSets; i++)
			{
				var values = GetInputData($"\tData Set {i + 1}: ");
				if (values == null)
				{
					PrintNewLine();
					return this;
				}

				var expectedResult = GetExpectedResult($"\tExpected Result for Data Set {i + 1}: ");
				if (expectedResult == null)
				{
					PrintNewLine();
					return this;
				}

				newDatasets.Add(new NNDataSet(values, expectedResult));
			}

			_dataSets = newDatasets;
			PrintNewLine();
			return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets input data. </summary>
        ///
        /// <param name="message">  The message. </param>
        ///
        /// <returns>   An array of double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  double[] GetInputData(string message)
		{
            Console.Write(message, Color.Yellow);
			var line = GetLine();

			if (line.Equals("menu", StringComparison.InvariantCultureIgnoreCase)) 
			    return null;

			while (line == null || line.Split(' ').Length != _numInputParameters)
			{
				Console.WriteLine($"\t{_numInputParameters} inputs are required.", Color.Red);
				PrintNewLine();
				Console.WriteLine(message);
				line = GetLine();
			}

			var values = new double[_numInputParameters];
			var lineNums = line.Split(' ');
			for (var i = 0; i < lineNums.Length; i++)
			{
			    if (double.TryParse(lineNums[i], out var num))
				{
					values[i] = num;
				}
				else
				{
					Console.WriteLine("\tYou entered an invalid number.  Try again", Color.Red);
					PrintNewLine(2);
					return GetInputData(message);
				}
			}

			return values;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets expected result. </summary>
        ///
        /// <param name="message">  The message. </param>
        ///
        /// <returns>   An array of double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  double[] GetExpectedResult(string message)
		{
			Console.Write(message, Color.Yellow);
			var line = GetLine();

			if (line != null && line.Equals("menu", StringComparison.InvariantCultureIgnoreCase))
			    return null;

			while (line == null || line.Split(' ').Length != _numOutputParameters)
			{
				Console.WriteLine($"\t{_numOutputParameters} outputs are required.", Color.Red);
				PrintNewLine();
				Console.WriteLine(message);
				line = GetLine();
			}

			var values = new double[_numOutputParameters];
			var lineNums = line.Split(' ');
			for (var i = 0; i < lineNums.Length; i++)
			{
			    if (int.TryParse(lineNums[i], out var num) && (num == 0 || num == 1))
				{
					values[i] = num;
				}
				else
				{
					Console.WriteLine("\tYou must enter 1s and 0s!", Color.Red);
					PrintNewLine(2);
					return GetExpectedResult(message);
				}
			}

			return values;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests network. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  NNManager TestNetwork()
		{
            Console.WriteLine("\tTesting Network", Color.Yellow);
			PrintNewLine();

			while (true)
			{
				PrintUnderline(50);
				var values = GetInputData($"\tType {_numInputParameters} inputs: ");
				if (values == null)
				{
					PrintNewLine();
					return this;
				}

				var results = _network?.Compute(values);
				PrintNewLine();

				foreach (var result in results)
				{
                    Console.WriteLine("\tOutput: " + DoubleConverter.ToExactString(result), Color.Aqua);
				}

				PrintNewLine();
			    return this;
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Train network to minimum. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public  NNManager TrainNetworkToMinimum()
        {
            var minError = GetDouble("\tMinimum Error: ", 0.000000001, 1.0);
            PrintNewLine();
            Console.WriteLine("\tTraining...");
            _network.Train(_dataSets, minError);
            Console.WriteLine("\t**Training Complete**", Color.Green);
            PrintNewLine();
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Train network to maximum. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public  NNManager TrainNetworkToMaximum()
        {
            var maxEpoch = GetInput("\tMax Epoch: ", 1, int.MaxValue);
            if (!maxEpoch.HasValue)
            {
                PrintNewLine();
                return this;
            }
            PrintNewLine();
            Console.WriteLine("\tTraining...");
            _network.Train(_dataSets, maxEpoch.Value);
            Console.WriteLine("\t**Training Complete**", Color.Green);
            PrintNewLine();
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Train network. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NNManager TrainNetwork()
		{
            Console.WriteLine("Network Training", Color.Yellow);
			PrintUnderline(50);
			Console.WriteLine("\t1. Train to minimum error", Color.Yellow);
			Console.WriteLine("\t2. Train to max epoch", Color.Yellow);
			Console.WriteLine("\t3. Network Menu", Color.Yellow);
			PrintNewLine();
			
		    switch (GetInput("\tYour Choice: ", 1, 3))
			{
				case 1:
                    TrainNetworkToMinimum();
					break;
				case 2:
                    TrainNetworkToMaximum();
					break;
				case 3:
					break;
			}
			PrintNewLine();
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import network. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public
		    NNManager ImportNetwork()
		{
			PrintNewLine();
			_network = ImportHelper.ImportNetwork();
			if (_network == null)
			{
				WriteError("\t****Something went wrong while importing your network.****");
				return this;
			}

			_numInputParameters = _network.InputLayer.Count;
			_hiddenNeurons = new int[_network.HiddenLayers.Count];
			_numOutputParameters = _network.OutputLayer.Count;

			Console.WriteLine("\t**Network successfully imported.**", Color.Green);
			PrintNewLine();
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Export network. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  NNManager ExportNetwork()
		{
			PrintNewLine();
			Console.WriteLine("\tExporting Network...");
			ExportHelper.ExportNetwork(_network);
			Console.WriteLine("\t**Exporting Complete!**", Color.Green);
			PrintNewLine();
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import datasets. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager ImportDatasets(string name)
		{
			PrintNewLine();
			_dataSets = ImportHelper.ImportDatasets(name);
			if (_dataSets == null)
			{
				WriteError("\t--Something went wrong while importing your datasets.--");
				return this;
			}

			if (_dataSets.Any(x => x.Values.Length != _numInputParameters || _dataSets.Any(y => y.Targets.Length != _numOutputParameters)))
			{
				WriteError($"\t--The dataset does not fit the network.  Network requires datasets that have {_numInputParameters} inputs and {_numOutputParameters} outputs.--");
				return this;
			}

			Console.WriteLine("\t**Datasets successfully imported.**", Color.Green);
			PrintNewLine();
		    return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Export datasets. </summary>
        ///
        /// <returns>   A NNManager. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public NNManager ExportDatasets()
		{
			PrintNewLine();
			Console.WriteLine("\tExporting Datasets...");
			ExportHelper.ExportDatasets(_dataSets);
			Console.WriteLine("\t**Exporting Complete!**", Color.Green);
			PrintNewLine();
            return this;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the line. </summary>
        ///
        /// <returns>   The line. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  string GetLine()
		{
			var line = Console.ReadLine();
			return line?.Trim() ?? string.Empty;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an input. </summary>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="min">      The minimum. </param>
        /// <param name="max">      The maximum. </param>
        ///
        /// <returns>   The input. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  int? GetInput(string message, int min, int max)
		{
			Console.Write(message);
			var num = GetNumber();
			if (!num.HasValue) return null;

			while (!num.HasValue || num < min || num > max)
			{
                Console.Write(message, Color.Red);
				num = GetNumber();
			}

			return num.Value;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the double. </summary>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="min">      The minimum. </param>
        /// <param name="max">      The maximum. </param>
        ///
        /// <returns>   The double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  double GetDouble(string message, double min, double max)
		{
			Console.Write(message);
			var num = GetDouble();

			while (num < min || num > max)
			{
				Console.Write(message, Color.Red);
				num = GetDouble();

			}

			return num;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets array input. </summary>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="min">      The minimum. </param>
        /// <param name="numToGet"> Number of to gets. </param>
        ///
        /// <returns>   An array of int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  int[] GetArrayInput(string message, int min, int numToGet)
		{
			var nums = new int[numToGet];

			for (var i = 0; i < numToGet; i++)
			{
				Console.Write(message + " " + (i + 1) + ": ");
				var num = GetNumber();

				while (!num.HasValue || num < min)
				{
                    Console.Write(message + " " + (i + 1) + ": ");
					num = GetNumber();
				}

				nums[i] = num.Value;
			}

			return nums;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the number. </summary>
        ///
        /// <returns>   The number. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  int? GetNumber()
		{
		    var line = GetLine();

			if (line.Equals("menu", StringComparison.InvariantCultureIgnoreCase)) 
			    return null;

			return int.TryParse(line, out var num) ? num : 0;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the double. </summary>
        ///
        /// <returns>   The double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  double GetDouble()
		{
		    var line = GetLine();
			return line != null && double.TryParse(line, out var num) ? num : 0;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Print new line. </summary>
        ///
        /// <param name="numNewLines">  (Optional) Number of new lines. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  void PrintNewLine(int numNewLines = 1)
		{
			for (var i = 0; i < numNewLines; i++)
				Console.WriteLine();
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Print underline. </summary>
        ///
        /// <param name="numUnderlines">    Number of underlines. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  void PrintUnderline(int numUnderlines)
		{
			for (var i = 0; i < numUnderlines; i++)
				Console.Write("-", Color.Yellow);
			PrintNewLine(2);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes an error. </summary>
        ///
        /// <param name="error">    The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public  void WriteError(string error)
		{
			Console.WriteLine(error, Color.Red);
			Exit();
		}

        /// <summary>   Exits this object. </summary>
		public  void Exit()
		{
			Console.WriteLine("Exiting...", Color.Yellow);
			Console.ReadLine();
			Environment.Exit(0);
		}

    }
}
