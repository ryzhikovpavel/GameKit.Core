using System.Reflection;
using UnityEngine;

namespace GameKit.Csv
{
    public static class SpreadsheetReflection
    {
        public static void ApplyTo(this Spreadsheet spreadsheet, int rowIndex, object to)
        {
            var type = to.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                int index = spreadsheet.GetColumnIndex(field.Name);
                if (index < 0) continue;

                string strValue = spreadsheet[index, rowIndex];

                if (field.FieldType == typeof(int))
                    field.SetValue(to, int.Parse(strValue));
                else if (field.FieldType == typeof(float))
                    field.SetValue(to, float.Parse(strValue));
                else if (field.FieldType == typeof(double))
                    field.SetValue(to, double.Parse(strValue));
                else if (field.FieldType == typeof(string))
                    field.SetValue(to, strValue);
            }
        }
    }
}