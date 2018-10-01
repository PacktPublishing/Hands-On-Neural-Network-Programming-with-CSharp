using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Helpers
{
    using System.Globalization;

    /// <summary>Private class used for manipulating
    class ArbitraryDecimal
    {
        /// <summary>Digits in the decimal expansion, one byte per digit </summary> 
        byte[] digits;

        /// <summary>How many digits are *after* the decimal point</summary>
        int decimalPoint = 0;

        /// <summary> 
        /// Constructs an arbitrary decimal expansion from the given long.
        /// The long must not be negative.
        /// </summary>
        internal ArbitraryDecimal(long x)
        {
            string tmp = x.ToString(CultureInfo.InvariantCulture);
            digits = new byte[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
                digits[i] = (byte)(tmp[i] - '0');
            Normalize();
        }

        /// <summary>
        /// Multiplies the current expansion by the given amount, which should
        /// only be 2 or 5.
        /// </summary>
        internal void MultiplyBy(int amount)
        {
            byte[] result = new byte[digits.Length + 1];
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int resultDigit = digits[i] * amount + result[i + 1];
                result[i] = (byte)(resultDigit / 10);
                result[i + 1] = (byte)(resultDigit % 10);
            }
            if (result[0] != 0)
            {
                digits = result;
            }
            else
            {
                Array.Copy(result, 1, digits, 0, digits.Length);
            }
            Normalize();
        }

        /// <summary>
        /// Shifts the decimal point; a negative value makes
        /// the decimal expansion bigger (as fewer digits come after the
        /// decimal place) and a positive value makes the decimal
        /// expansion smaller.
        /// </summary>
        internal void Shift(int amount)
        {
            decimalPoint += amount;
        }

        /// <summary>
        /// Removes leading/trailing zeroes from the expansion.
        /// </summary>
        internal void Normalize()
        {
            int first;
            for (first = 0; first < digits.Length; first++)
                if (digits[first] != 0)
                    break;
            int last;
            for (last = digits.Length - 1; last >= 0; last--)
                if (digits[last] != 0)
                    break;

            if (first == 0 && last == digits.Length - 1)
                return;

            byte[] tmp = new byte[last - first + 1];
            for (int i = 0; i < tmp.Length; i++)
                tmp[i] = digits[i + first];

            decimalPoint -= digits.Length - (last + 1);
            digits = tmp;
        }

        /// <summary>
        /// Converts the value to a proper decimal string representation.
        /// </summary>
        public override string ToString()
        {
            char[] digitString = new char[digits.Length];
            for (int i = 0; i < digits.Length; i++)
                digitString[i] = (char)(digits[i] + '0');

            // Simplest case - nothing after the decimal point,
            // and last real digit is non-zero, eg value=35
            if (decimalPoint == 0)
            {
                return new string(digitString);
            }

            // Fairly simple case - nothing after the decimal
            // point, but some 0s to add, eg value=350
            if (decimalPoint < 0)
            {
                return new string(digitString) + new string('0', -decimalPoint);
            }

            // Nothing before the decimal point, eg 0.035
            if (decimalPoint >= digitString.Length)
            {
                return "0." + new string('0', (decimalPoint - digitString.Length)) + new string(digitString);
            }

            // Most complicated case - part of the string comes
            // before the decimal point, part comes after it,
            // eg 3.5
            return new string(digitString, 0, digitString.Length - decimalPoint) + "." +
                new string(digitString, digitString.Length - decimalPoint, decimalPoint);
        }
    }
}
