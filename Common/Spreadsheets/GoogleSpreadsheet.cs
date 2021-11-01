using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace GameKit
{
    [Serializable]
    public class GoogleSpreadsheet
    {
        public const string Url = "https://docs.google.com/spreadsheets/d/";

        public bool IsLoaded { get; private set; }
        public Spreadsheet Data { get; private set; }
        
        [SerializeField] private string _grid;
        [SerializeField] private string _table;


        public GoogleSpreadsheet(string url)
        {
            url = url.Remove(0, Url.Length);
            _table = url.Substring(0, url.IndexOf("/"));
            _grid = url.Substring(url.IndexOf("gid=") + 4, url.Length - url.IndexOf("gid=") - 4);
        }

        public string ExportToCvs()
        {
            return string.Format("{0}{1}/export?format=csv&gid={2}&rand={3}", Url, _table, _grid, UnityEngine.Random.value);
        }

        public IEnumerator Load()
        {
            var uwr = UnityWebRequest.Get(ExportToCvs());
            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error) == false)
            {
                throw new Exception(uwr.error);
            }

            Data = new Spreadsheet(new CsvReader(uwr.downloadHandler.text));
            IsLoaded = true;
        }
    }
}