using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameKit.Csv
{
    public class SpreadsheetAttribute : Attribute
    {
        public readonly string Name;
        public readonly int RowIndex;

        public SpreadsheetAttribute(string name, int rowIndex = 0)
        {
            Name = name;
            RowIndex = rowIndex;
        }
    }
    
    
    public static class SpreadsheetReflection
    {
        private static Dictionary<Type, Func<string, object>> _supportedTypes;
        private static Dictionary<Type, Func<string, object>> Initialize()
        {
            return new Dictionary<Type, Func<string, object>>()
            {
                { typeof(int), (str) => int.Parse(str) },
                { typeof(float), (str) => float.Parse(str) },
                { typeof(double), (str) => double.Parse(str) },
                { typeof(string), (str) => str },
            };
        }

        public static Dictionary<Type, Func<string, object>> SupportedTypes => _supportedTypes ??= Initialize();

        public static void ApplyTo(this Spreadsheet spreadsheet, int rowIndex, object to)
        {
            if (_supportedTypes is null) _supportedTypes = Initialize();
            
            var type = to.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                string fieldName = field.Name;
                int headerIndex = 0;
                var spa = field.GetCustomAttribute<SpreadsheetAttribute>();
                if (spa != null)
                {
                    fieldName = spa.Name;
                    headerIndex = spa.RowIndex;
                }

                int index = spreadsheet.GetColumnIndex(fieldName, headerIndex);
                if (index < 0) continue;

                string strValue = spreadsheet[index, rowIndex];
                if (string.IsNullOrEmpty(strValue)) continue;

                Type fieldType = field.FieldType;

                try
                {
                    if (fieldType.IsEnum)
                        field.SetValue(to, Enum.Parse(fieldType, strValue));
                
                    // ReSharper disable once PossibleNullReferenceException
                    if (_supportedTypes.TryGetValue(fieldType, out var convertFunc))
                        field.SetValue(to, convertFunc(strValue));
                }
                catch (Exception e)
                {
                    throw new Exception($"'{fieldName}' field in {rowIndex} row with {strValue} content parse failed", e);
                }
            }
        }
    }
}