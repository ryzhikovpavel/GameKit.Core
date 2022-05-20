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
            EditorUtility.DisplayProgressBar("Localization updating", "Loading google spreadsheet data", 0);
            
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

            try
            {
                EditorUtility.DisplayProgressBar("Localization updating", "Creating translation config files", 0.7f);

                CreateDataLanguageTranslation(localization, spreadsheets);
            
                EditorUtility.DisplayProgressBar("Localization updating", "Appling translation data", 0.85f);
                ApplySpreadsheets(localization, spreadsheets);

                GenerateTranslationClass(localization.Default);

                EditorUtility.SetDirty(localization);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log("Localization compile successful");
            }
            catch (Exception e)
            {
                //ignore
                Debug.Log("Localization compile with errors");
                Debug.LogException(e);
            }
            
            EditorUtility.ClearProgressBar();
        }

        private static void GenerateTranslationClass(DataLanguageTranslation translation)
        {
            var builder = new ConstantFileBuilder("Translation");

            foreach (var t in translation.Translations)
            {
                builder.Add(t.key, t.key, t.value);
            }
            
            builder.Save(Path.Combine(Application.dataPath, "Sources", "Generated"));
        }

        private static void CreateDataLanguageTranslation(DataLocalization localization, Spreadsheet[] spreadsheets)
        {
            string path = Path.Combine(Application.dataPath, "Resources/Translations/");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);

            foreach (var spreadsheet in spreadsheets)
            {
                foreach (var key in spreadsheet.Columns)
                {
                    if (key.ToLower() == "key") continue;
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
    }
}