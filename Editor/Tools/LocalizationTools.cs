using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameKit.Csv;
using GameKit.Implementation;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public static class LocalizationTools
    {
        [MenuItem("GameKit/Localization/Load + Update")]
        private static async void LoadLocalization()
        {
            var localization = Resources.Load<DataLocalization>("Localization");

            Task<Spreadsheet>[] requests = new Task<Spreadsheet>[localization.Spreadsheets.Length];

            for (var i = 0; i < localization.Spreadsheets.Length; i++)
                requests[i] = localization.Spreadsheets[i].LoadAsync();

            await Task.WhenAll(requests);
            
            Debug.Log("Loading successful");

            Spreadsheet[] spreadsheets = new Spreadsheet[requests.Length];
            for (var i = 0; i < requests.Length; i++)
            {
                var task = requests[i];
                if (task.IsCompleted)
                {
                    spreadsheets[i] = task.Result;
                }
                else
                {
                    Debug.LogException(task.Exception);
                    throw new Exception("Loading failed");
                }
            }

            CreateDataLanguageTranslation(localization, spreadsheets);
            ApplySpreadsheets(localization, spreadsheets);
            Debug.Log("Localization compile successful");
        }

        private static void CreateDataLanguageTranslation(DataLocalization localization, Spreadsheet[] spreadsheets)
        {
            string path = Path.Combine(Application.dataPath, "Resources/Translations/");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);

            foreach (var spreadsheet in spreadsheets)
            {
                foreach (var key in spreadsheet.Columns)
                {
                    if (key.ToLowerInvariant() == "key") continue;
                    if (File.Exists(path + key + ".asset")) continue;

                    var asset = ScriptableObject.CreateInstance<DataLanguageTranslation>();
                    asset.Set(Enum.Parse<SystemLanguage>(key, true));
                    AssetDatabase.CreateAsset(asset, "Assets/Resources/Translations/"+key+".asset");
                    localization.AddTranslation(asset);
                }
            }
        }
        
        private static void ApplySpreadsheets(DataLocalization localization, Spreadsheet[] spreadsheets)
        {
            foreach (var translation in localization.Translations)
            {
                ApplyLanguage(translation, spreadsheets, "key", translation.Language.ToString().ToLower());
                EditorUtility.SetDirty(translation);
            }

            if (localization.Translations.Length > 0)
                GenerateLocalizationConstants(localization.Translations[0].Translations);

            EditorUtility.SetDirty(localization);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ApplyLanguage(DataLanguageTranslation translation, Spreadsheet[] spreadsheets, string keyColumn, string valueColumn)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();

            foreach (var spreadsheet in spreadsheets)
            {
                var keyIndex = spreadsheet.GetColumnIndex(keyColumn);
                if (keyIndex < 0) throw new Exception($"Key '{keyColumn}' not found");
                var valueIndex = spreadsheet.GetColumnIndex(valueColumn);
                if (valueIndex < 0) throw new Exception($"Key '{valueColumn}' not found");
                
                for (int i = 1; i < spreadsheet.Rows.Length; i++)
                {
                    if (string.IsNullOrEmpty(spreadsheet[keyIndex, i])) continue;
                    if (translations.ContainsKey(spreadsheet[keyIndex, i]))
                        Debug.LogError($"duplicate: key({spreadsheet[keyIndex, i]}) value({spreadsheet[valueIndex, i]})");
                    translations.Add(spreadsheet[keyIndex, i], spreadsheet[valueIndex, i]);
                }
            }

            translation.Set(translations);
        }

        //[MenuItem("Tools/GenerateConstants")]
        private static void GenerateConstants()
        {
            GenerateLocalizationConstants(Resources.Load<DataLocalization>("Localization").Default.Translations);
        }

        private static void GenerateLocalizationConstants(DataLanguageTranslation.Translation[] translations)
        {
            Dictionary<string, DataLanguageTranslation.Translation> constants = new Dictionary<string, DataLanguageTranslation.Translation>();
            char[] separators = { ' ', '_', '-' };

            foreach (var translation in translations)
            {
                string[] parts = translation.key.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                string key = "";
                foreach (var part in parts)
                {
                    key += FixCamelName(part);
                }
                constants.Add(key, translation);
            }

            var path = Path.Combine(Application.dataPath, "GameKit", "Generated");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "Translation.cs");

            separators = new[] { '\n' };

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace Sources");
            builder.AppendLine("{");
            builder.AppendLine("\tpublic class Translation");
            builder.AppendLine("\t{");
            foreach (var constant in constants)
            {
                builder.AppendLine($"\t\t/// <summary>");
                var parts = constant.Value.value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    builder.AppendLine($"\t\t/// {part}");
                }
                builder.AppendLine($"\t\t/// </summary>");
                builder.AppendLine($"\t\tpublic const string {constant.Key} = \"{constant.Value.key}\";");
            }
            builder.AppendLine("\t}");
            builder.AppendLine("}");

            File.WriteAllText(path, builder.ToString());
        }

        public static string FixCamelName(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            char[] symbols = new char[]
            {
                'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x',
                'c', 'v', 'b', 'n', 'm'
            };
            char[] numbers = new char[]
            {
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
            };

            char[] a = s.ToLower().ToCharArray();

            for (int i = a.Length - 1; i >= 0; i--)
            {
                char c = a[i];
                if (symbols.Contains(c)) continue;
                if (numbers.Contains(c)) continue;
                ArrayTools.Remove(ref a, i);
            }

            if (a.Length == 0)
            {
                Debug.LogError($"Key '{s}' is not valid");
                return s;
            }
            
            if (numbers.Contains(a[0]))
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
        
        // [MenuItem("Tools/Localization/Test")]
        // private static void TestLoc()
        // {
        //     string[] guids = new string[] {"c4b2a5bd5bf381749b16837442783341", "83655f3cf0f5c3f4c9a332407144fecd", "c658e06e3044b204b98a2dcb7f8d6721"};
        //     List<string> linked = new List<string>();
        //     
        //     var files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
        //
        //     foreach (var file in files)
        //     {
        //         var lines = File.ReadAllLines(file);
        //         foreach (var guid in guids)
        //         {
        //             AppendLinkedKeys(linked, lines, guid);
        //         }    
        //     }
        //
        //     string output = string.Empty;
        //     foreach (var line in linked)
        //     {
        //         output += line + "\n";
        //         //Debug.Log(line);
        //     }
        //
        //     string pathToFile = Application.persistentDataPath + "/LocalizationTest.txt";
        //     File.WriteAllText(pathToFile, output);
        //     Application.OpenURL(pathToFile);
        // }
        //
        // private static void AppendLinkedKeys(List<string> linked, string[] lines, string guid)
        // {
        //     for (int i = 0; i < lines.Length;)
        //     {
        //         string line = lines[i];
        //         if (line.Contains(guid))
        //         {
        //             i += 3;
        //             line = lines[i];
        //             if (line.Contains("_key:") == false) throw new Exception($"Key not found in {i} line");
        //             linked.Add(line.Replace("_key:", "").Trim());
        //         }
        //         else i++;
        //     }
        // }
    }
}