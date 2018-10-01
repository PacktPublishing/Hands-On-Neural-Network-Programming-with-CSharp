using System;

namespace NeuralNetwork.NetworkModels
{
    /// <summary>   A sigmoid. </summary>
	public static class Sigmoid
	{
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Outputs. </summary>
        ///
        /// <param name="x">    The x coordinate. </param>
        ///
        /// <returns>   A double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static double Output(double x)
		{
			return x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Math.Exp(-x));
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Derivatives. </summary>
        ///
        /// <param name="x">    The x coordinate. </param>
        ///
        /// <returns>   A double. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static double Derivative(double x)
		{
			return x * (1 - x);
		}
	}
}