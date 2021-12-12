using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameKit.Csv
{
    [Serializable]
    public class GoogleSpreadsheet
    {
        public const string Url = "https://docs.google.com/spreadsheets/d/";
        
        [SerializeField] private string _grid;
        [SerializeField] private string _table;
        
        public GoogleSpreadsheet(string url)
        {
            url = url.Remove(0, Url.Length);
            _table = url.Substring(0, url.IndexOf("/"));
            _grid = url.Substring(url.IndexOf("gid=") + 4, url.Length - url.IndexOf("gid=") - 4);
        }

        public string ExportToCvsUrl()
        {
            return string.Format("{0}{1}/export?format=csv&gid={2}&rand={3}", Url, _table, _grid, UnityEngine.Random.value);
        }

        public async Task<Spreadsheet> LoadAsync()
        {
            using (var uwr = UnityWebRequest.Get(ExportToCvsUrl()))
            {
                uwr.SendWebRequest();
                while (uwr.isDone == false) await Task.Yield();
                
                switch (uwr.result)
                {
                    case UnityWebRequest.Result.Success: break;
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.DataProcessingError:
                        throw new Exception(uwr.error);
                    default: throw new ArgumentOutOfRangeException();
                }

                return new Spreadsheet(new CsvReader(uwr.downloadHandler.text));
            }
        }
    }
}