using System.Data;
/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-09
  Script Name: TestCommands.cs

  Description:
  This script implements test commands.
  Please uncomment INCLUDE_TESTS definition to enable tests.
  It should be uncommented in both ConsoleTests.cs and TestCommands.cs files.
  Note that this is by default disabled to not load tests commands.
*/

// uncomment this

using UnityEngine;

namespace ED.SC.Tests
{
    public class TestCommands : MonoBehaviour
    {
        [Command]
        public void print_hello_world()
        {
            SmartConsole.Log("Hello World!");
        }

        [Command]
        public void print_bool(bool state)
        {
            SmartConsole.Log(state.ToString());
        }

        [Command]
        public void print_bool_optional(bool state = true)
        {
            SmartConsole.Log(state.ToString());
        }

        public enum MyEnum
        {
            One = 0,
            Two = 1,
            Three = 2,
            Four = 4
        }

        [Command]
        public void print_enum(MyEnum myEnum)
        {
            SmartConsole.Log(myEnum.ToString());
        }

        [Command]
        public void print_enum_optional(MyEnum myEnum = MyEnum.Three)
        {
            SmartConsole.Log(myEnum.ToString());
        }

        [Command]
        public void print_float(float n)
        {
            SmartConsole.Log(n.ToString());
        }

        [Command]
        public void print_float_optional(float n = 1.2f)
        {
            SmartConsole.Log(n.ToString());
        }

        [Command]
        public void print_int(int n)
        {
            SmartConsole.Log(n.ToString());
        }

        [Command]
        public void print_int_optional(int n = 1)
        {
            SmartConsole.Log(n.ToString());
        }

        [Command]
        public void print_string(string message)
        {
            SmartConsole.Log($"{message}");
        }

        [Command]
        public void print_string_optional(string message = "success")
        {
            SmartConsole.Log($"{message}");
        }

        [Command]
        public void print_string_x_time(string str, int n)
        {
            for (int i = 0; i < n; i++)
            {
                SmartConsole.Log(str);
            }
        }

        [Command]
        public void print_strings_and_ints(string str1, int n1, string str2, int n2)
        {
            SmartConsole.Log($"{str1} {n1.ToString()} {str2} {n2.ToString()}");
        }
    }
}
