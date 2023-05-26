/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-27
  Script Name: TypeUtil.cs

  Description:
  This script implements utility functions for working with Type.
*/

using System;

namespace ED.SC
{
	public static class TypeUtil
	{
		private static int m_IntModifier = 1;
		private static float m_FloatModifier = 0.1f;

		public static string GetDefaultStringValue(Type type)
		{
			if (type == typeof(string))
			{
				return "message";
			}
			else if (type == typeof(int))
			{
				return default(int).ToString();
			}
			else if (type == typeof(bool))
			{
				return default(bool).ToString().ToLower();
			}
			else if (type == typeof(float))
			{
				return default(float).ToString();
			}
			else if (type.IsEnum)
			{
				return EnumUtils.GetFullValueName((Enum)Enum.GetValues(type).GetValue(0));
			}
			// write your custom types here
			// else if (type == typeof(myType))
			// {
			// 	return myDefaultValueType;
			// }
			else
			{
				throw new SmartParameterTypeNotSupportedException(type.Name);
			}
		}

		public static string GetStringValue(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}
			else if (type == typeof(int))
			{
				return value.ToString();
			}
			else if (type == typeof(bool))
			{
				return value.ToString().ToLower();
			}
			else if (type == typeof(float))
			{
				return value.ToString();
			}
			else if (type.IsEnum)
			{
				return EnumUtils.GetFullValueName((Enum)value);
			}
			// write your custom types here (optional - parameter autocomplete usage)
			// else if (type == typeof(myType))
			// {
			// 	// object to string
			// 	return myStringValue;
			// }
			else
			{
				throw new SmartParameterTypeNotSupportedException(type.Name);
			}
		}

		public static object GetObjectValue(Type type, string value)
		{
			if (type == typeof(string))
			{
				return value;
			}
			else if (type == typeof(int))
			{
				return StringUtil.ToInt(value);
			}
			else if (type == typeof(bool))
			{
				return StringUtil.ToBool(value);
			}
			else if (type == typeof(float))
			{
				return StringUtil.ToFloat(value);
			}
			else if (type.IsEnum)
			{
				return StringUtil.ToEnum(value, type);
			}
			// write your custom types here
			// else if (type == typeof(myType))
			// {
			// 	// string to object
			// 	return myObjectValue;
			// }
			else
			{
				throw new SmartParameterTypeNotSupportedException(type.Name);
			}
		}

		public static object GetNextObjectValue(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}
			else if (type == typeof(int))
			{
				return (int)value + m_IntModifier;
			}
			else if (type == typeof(bool))
			{
				return !(bool)value;
			}
			else if (type == typeof(float))
			{
				return (float)value + m_FloatModifier;
			}
			else if (type.IsEnum)
			{
				return EnumUtils.GetNextValue((Enum)value);
			}
			// write your custom types here (optional - parameter autocomplete usage)
			// else if (type == typeof(myType))
			// {
			// 	// myType's next value
			// 	return value++;
			// }
			else
			{
				throw new SmartParameterTypeNotSupportedException(type.Name);
			}
		}
	}
}
