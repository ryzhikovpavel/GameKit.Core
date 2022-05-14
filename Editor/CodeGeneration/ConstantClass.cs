using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace GameKit.Editor.Tools
{
    public class ConstantClass
    {
        private static readonly char[] Symbols = new char[]
        {
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x',
            'c', 'v', 'b', 'n', 'm', 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X',
            'C', 'V', 'B', 'N', 'M'
        };
        private static readonly char[] Numbers = new char[]
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };
        
        private static readonly char[] Separators = new char[]{ ' ', '_', '-' };
        
        protected readonly string ClassName;
        private readonly List<ConstantClass> _classes = new List<ConstantClass>();
        private readonly List<Item> _lines = new List<Item>();

        private struct Item
        {
            public readonly string Name;
            public readonly string Value;
            public readonly string Comment;

            public Item(string name, string value, string comment)
            {
                Name = name;
                Value = value;
                Comment = comment;
            }
        }

        protected ConstantClass( string className)
        {
            ClassName = GetCamelName(className);
        }
        
        public void Add(string prefererName, string value, string comment = null)
        {
            _lines.Add(new Item(GetCamelName(prefererName), value, comment));
        }
        
        public ConstantClass Add(string className)
        {
            var cls = new ConstantClass(className);
            _classes.Add(cls);
            return cls;
        }

        protected string GenerateCode(string offset)
        {
            string lineOffset = offset + '\t';
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{offset}public class {ClassName}");
            builder.AppendLine(offset+"{");

            foreach (var @class in _classes)
            {
                builder.AppendLine(@class.GenerateCode(lineOffset));
            }
            
            foreach (var line in _lines)
            {
                if (string.IsNullOrEmpty(line.Comment) == false)
                {
                    builder.AppendLine($"{lineOffset}/// <summary>");
                    string[] parts = line.Comment.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        builder.AppendLine($"{lineOffset}/// {part}");
                    }
                    builder.AppendLine($"{lineOffset}/// </summary>");
                }

                builder.AppendLine($"{lineOffset}public const string {line.Name} = \"{FixValue(line.Value)}\";");
            }
            builder.AppendLine(offset + "}");

            return builder.ToString();
        }

        private static string FixValue(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
        
        protected static string GetCamelName(string prefererName)
        {
            string[] parts = prefererName.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            string name = "";
            bool first = true;
            foreach (var part in parts)
            {
                name += FixCamelNamePart(part, first);
                first = false;
            }

            return name;
        }
        
        private static string FixCamelNamePart(string s, bool replaceFirstNumber = true)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            char[] a = s.ToCharArray();

            for (int i = a.Length - 1; i >= 0; i--)
            {
                char c = a[i];
                if (Symbols.Contains(c)) continue;
                if (Numbers.Contains(c)) continue;
                Remove(ref a, i);
            }

            if (a.Length == 0)
            {
                Debug.LogError($"Key '{s}' is not valid");
                return s;
            }
            
            if (replaceFirstNumber && Numbers.Contains(a[0]))
            {
                Array.Resize(ref a, a.Length + 1);
                for (int i = a.Length - 1; i >= 1; i--)
                {
                    a[i] = a[i - 1];
                }

                a[0] = 'C';
            } else a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
        
        protected static void Remove<T>(ref T[] array, int index)
        {
            for (int i = index; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            Array.Resize(ref array, array.Length - 1);
        }
    }
}