/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: CommandAttribute.cs

  Description:
  This class implements the CommandAttribute attribute which can be applied
  to a method to indicate that it should be registered as a command.
*/

using System;

namespace ED.SC
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
		public string Name;
		public string Description;
		public MonoTargetType MonoTargetType;

		public CommandAttribute() : this("", "", MonoTargetType.Single)
		{
		}

		public CommandAttribute(string description) : this("", description, MonoTargetType.Single)
		{
		}

		public CommandAttribute(string name, string description) : this(name, description, MonoTargetType.Single)
		{
		}

		public CommandAttribute(string description, MonoTargetType monoTargetType) : this("", description, monoTargetType)
		{
		}

		public CommandAttribute(MonoTargetType monoTargetType) : this("", "", monoTargetType)
		{
		}

		public CommandAttribute(string name, string description, MonoTargetType monoTargetType)
		{
			Name = name;
			Description = description;
			MonoTargetType = monoTargetType;
		}

		public bool HasName() => !string.IsNullOrEmpty(Name);
	}
}