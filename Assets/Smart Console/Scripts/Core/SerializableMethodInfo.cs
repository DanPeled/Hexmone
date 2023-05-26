/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-06
  Script Name: SerializableMethodInfo.cs

  Description:
  This class is used to serialize method info.
*/

using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

namespace ED.SC
{
	[Serializable]
	public class SerializableMethodInfo : ISerializationCallbackReceiver
	{
		public MethodInfo MethodInfo;
		public SerializableType Type;
		public string MethodName;
		public List<SerializableType> Parameters = null;
		public int Flags = 0;

		public SerializableMethodInfo(MethodInfo methodInfo)
		{
			MethodInfo = methodInfo;
		}

		public void OnBeforeSerialize()
		{
			if (MethodInfo == null)
			{
				return;
			}

			Type = new SerializableType(MethodInfo.DeclaringType);
			MethodName = MethodInfo.Name;

			if (MethodInfo.IsPrivate)
			{
				Flags |= (int)BindingFlags.NonPublic;
			}
			else
			{
				Flags |= (int)BindingFlags.Public;
			}

			if (MethodInfo.IsStatic)
			{
				Flags |= (int)BindingFlags.Static;
			}
			else
			{
				Flags |= (int)BindingFlags.Instance;
			}

			ParameterInfo[] parameters = MethodInfo.GetParameters();

			if (parameters != null && parameters.Length > 0)
			{
				Parameters = new List<SerializableType>(parameters.Length);

				for (int i = 0; i < parameters.Length; i++)
				{
					Parameters.Add(new SerializableType(parameters[i].ParameterType));
				}
			}
			else
			{
				Parameters = null;
			}
		}

		public void OnAfterDeserialize()
		{
			if (Type == null || string.IsNullOrEmpty(MethodName))
			{
				return;
			}

			Type type = Type.Type;
			Type[] parameterTypes = null;

			if (type == null)
			{
				Debug.LogError($"Smart Error: The type containing method '{MethodName}' cannot be found. Please reload the cache.");
				return;
			}

			if (Parameters != null && Parameters.Count > 0)
			{
				parameterTypes = new Type[Parameters.Count];

				for (int i = 0; i < Parameters.Count; i++)
				{
					parameterTypes[i] = Parameters[i].Type;
				}
			}

			if (parameterTypes == null)
			{
				MethodInfo = type.GetMethod(MethodName, (BindingFlags)Flags);
			}
			else
			{
				MethodInfo = type.GetMethod(MethodName, (BindingFlags)Flags, null, parameterTypes, null);
			}
		}
	}
}
