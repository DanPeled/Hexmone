/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-27
  Script Name: StringUtil.cs

  Description:
  This script implements utility functions for working with strings.
*/

using System;
using System.Linq;

namespace ED.SC
{
	public static class StringUtil
	{
		/// <summary>
		/// Parses a string to an integer.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>The parsed integer.</returns>
		public static int ToInt(string input)
		{
			if (int.TryParse(input, out int result))
			{
				return result;
			}

			throw new InvalidCastException();
		}

		/// <summary>
		/// Parses a string to a float.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>The parsed float.</returns>
		public static float ToFloat(string input)
		{
			input = input.Replace('.', ',');

			if (float.TryParse(input, out float result))
			{
				return result;
			}

			throw new InvalidCastException();
		}


		/// <summary>
		/// Parses a string to a boolean.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>The parsed boolean.</returns>
		public static bool ToBool(string input)
		{
			if (bool.TryParse(input, out bool result))
			{
				return result;
			}

			throw new InvalidCastException();
		}

		/// <summary>
		/// Parses a string to an enumeration.
		/// </summary>
		/// <param name="input">The input string to parse to enumeration.</param>
		/// <param name="type">The type of the enumeration to parse to.</param>
		/// <returns>The parsed enumeration.</returns>
		public static Enum ToEnum(string input, Type type)
		{
			int pointCount = input.Count(c => c == '.');

			if (pointCount > 1)
			{
				throw new InvalidCastException();
			}

			if (pointCount == 1)
			{
				input = input.Substring(input.IndexOf('.') + 1);
			}

			string[] enumNames = Enum.GetNames(type);
			Array enumValues = Enum.GetValues(type);

			for (int i = 0; i < enumNames.Length; i++)
			{
				if (string.Equals(enumNames[i], input))
				{
					return (Enum)enumValues.GetValue(i);
				}
			}

			throw new InvalidCastException();
		}

		/// <summary>
		/// Replaces the word at a specified index in a string with a new word.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="wordIndex">The index of the word to replace.</param>
		/// <param name="newWord">The new word to replace the original word with.</param>
		/// <returns>The input string with the specified word replaced.</returns>
		public static string ReplaceWordAtIndex(string input, int wordIndex, string newWord)
		{
			string[] words = input.Split(' ');

			if (wordIndex < 0 || wordIndex >= words.Length)
			{
				throw new ArgumentOutOfRangeException("wordIndex", "The word index must be within the range of the input string.");
			}

			words[wordIndex] = newWord;

			return string.Join(" ", words);
		}
	}
}
