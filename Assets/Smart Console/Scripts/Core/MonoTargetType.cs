/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-12
  Script Name: MonoTargetType.cs

  Description:
  This script contains the enum definition for the different
  targeting behaviour of monobehaviour instance in the Smart Console.
  Do not concern static target type.
*/

namespace ED.SC
{
	public enum MonoTargetType
	{
		/// <summary>
		/// Targets the first active instance found of the MonoBehaviour.
		/// </summary>
		Single,
		/// <summary>
		/// Targets all active instances found of the MonoBehaviour.
		/// </summary>
		Active,
		/// <summary>
		/// Targets both active and inactive instances found of the MonoBehaviour.
		/// </summary>
		All
	}
}
