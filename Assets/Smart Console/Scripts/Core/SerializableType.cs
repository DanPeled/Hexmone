/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-06
  Script Name: SerializableType.cs

  Description:
  This class is used to serialize any type.
*/

using System;
using System.IO;
using UnityEngine;

namespace ED.SC
{
	[Serializable]
	public class SerializableType : ISerializationCallbackReceiver
	{
		public Type Type;
		public byte[] Data;

		public SerializableType(Type type)
		{
			Type = type;
		}

		public static Type Read(BinaryReader reader)
		{
			var paramCount = reader.ReadByte();

			if (paramCount == 0xFF)
			{
				return null;
			}

			string typeName = reader.ReadString();
			Type type = Type.GetType(typeName);

			if (type == null)
			{
				throw new Exception("Can't find type; '" + typeName + "'");
			}

			if (type.IsGenericTypeDefinition && paramCount > 0)
			{
				Type[] parameterTypes = new Type[paramCount];

				for (int i = 0; i < paramCount; i++)
				{
					parameterTypes[i] = Read(reader);
				}

				type = type.MakeGenericType(parameterTypes);
			}

			return type;
		}

		public static void Write(BinaryWriter writer, Type type)
		{
			if (type == null)
			{
				writer.Write((byte)0xFF);
				return;
			}

			if (type.IsGenericType)
			{
				Type genericType = type.GetGenericTypeDefinition();
				Type[] parameterTypes = type.GetGenericArguments();

				writer.Write((byte)parameterTypes.Length);
				writer.Write(genericType.AssemblyQualifiedName);

				for (int i = 0; i < parameterTypes.Length; i++)
				{
					Write(writer, parameterTypes[i]);
				}

				return;
			}

			writer.Write((byte)0);
			writer.Write(type.AssemblyQualifiedName);
		}


		public void OnBeforeSerialize()
		{
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				Write(writer, Type);
				Data = stream.ToArray();
			}
		}

		public void OnAfterDeserialize()
		{
			if (Data == null || Data.Length == 0)
			{
				Type = null;
				return;
			}

			using (var stream = new MemoryStream(Data))
			using (var reader = new BinaryReader(stream))
			{
				Type = Read(reader);
			}
		}
	}
}