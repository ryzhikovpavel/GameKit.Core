using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameKit
{
    public class Spreadsheet
    {
        public Spreadsheet(CsvReader reader)
        {
            data = reader.data;
            rows = new string[data.GetLength(1)];
            columns = new string[data.GetLength(0)];

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = data[0, i];
            }

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = data[i, 0];
            }
        }

        public readonly string[] rows;
        public readonly string[] columns;
        private string[,] data;

        public string this[string c, string r]
        {
            get
            {
                int ir = rows.IndexOf(r);
                int ic = columns.IndexOf(c);
                return this[ic, ir];
            }
        }

        public string this[string c, int r]
        {
            get
            {                
                int ic = columns.IndexOf(c);
                return this[ic, r];
            }
        }

        public string this[int c, string r]
        {
            get
            {
                int ir = rows.IndexOf(r);
                return this[c, ir];
            }
        }

        public string this[int c, int r]
        {
            get
            {
                if (ValidateIndex(c, r))
                    return data[c, r];
                else return "";
            }
            set
            {
                if (ValidateIndex(c,r))
                    data[c,r] = value;
            }
        }

        public List<string> GetRow(int r, bool trim)
        {
            List<string> res = new List<string>(columns.Length);
            for (int i = 0; i < columns.Length; i++)
            {
                if (trim && string.IsNullOrEmpty(data[i, r])) continue;
                res.Add(data[i, r]);
            }
            return res;
        }
        public List<string> GetColumn(int c, bool trim)
        {
            List<string> res = new List<string>(rows.Length);
            for (int i = 0; i < rows.Length; i++)
            {
                if (trim && string.IsNullOrEmpty(data[c, i])) continue;
                res.Add(data[c,i]);
            }
            return res;
        }

        private bool ValidateIndex(int c, int r)
        {
            return r > 0 || r < data.GetLength(1) && c > 0 && c < data.GetLength(0);
        }


        public int GetColumnIndex(string key, int row = 0)
        {
            int len = data.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                if (key == data[i, row]) return i;
            }
            return -1;
        }

        public int GetRowIndex(string key, int col = 0)
        {
            int len = data.GetLength(1);
            for (int i = 0; i < len; i++)
            {
                if (key == data[col, i]) return i;
            }
            return -1;
        }

        public void Trim()
        {
            bool empty = true;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                empty = string.IsNullOrEmpty(data[i, 0]);
                if (!empty)
                    break;
            }
            
        }        
    }
}