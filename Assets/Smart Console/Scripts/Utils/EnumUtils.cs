/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-27
  Script Name: EnumUtils.cs

  Description:
  This script implements utility functions for working with Enum.
*/

using System;

namespace ED.SC
{
	public static class EnumUtils
	{
		/// <summary>
		/// Gets the next value in the Enum. If the current value is the last value, it will return the first value.
		/// </summary>
		/// <param name="value">The Enum value</param>
		/// <returns>The next Enum value</returns>
		public static Enum GetNextValue(Enum value)
		{
			Array values = Enum.GetValues(value.GetType());
			int index = Array.IndexOf(values, value);

			if (index == values.Length - 1)
			{
				return (Enum)values.GetValue(0);
			}

			return (Enum)values.GetValue(index + 1);
		}

		/// <summary>
		/// Gets the full name value of an Enum.
		/// </summary>
		/// <param name="value">The Enum value</param>
		/// <returns>The fulle name value</returns>
		public static string GetFullValueName(Enum value)
		{
			return $"{value.GetType().Name}.{value}";
		}
	}
}
