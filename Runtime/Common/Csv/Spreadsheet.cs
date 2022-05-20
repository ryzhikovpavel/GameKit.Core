using System.Collections.Generic;

namespace GameKit.Csv
{
    public class Spreadsheet
    {
        public Spreadsheet(CsvReader reader)
        {
            _data = reader.Data;
            Rows = new string[_data.GetLength(1)];
            Columns = new string[_data.GetLength(0)];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = _data[0, i];
            }

            for (int i = 0; i < Columns.Length; i++)
            {
                Columns[i] = _data[i, 0];
            }
        }

        public readonly string[] Rows;
        public readonly string[] Columns;
        private readonly string[,] _data;

        public string this[string c, string r]
        {
            get
            {
                int ir = Rows.IndexOf(r);
                int ic = Columns.IndexOf(c);
                return this[ic, ir];
            }
        }

        public string this[string c, int r]
        {
            get
            {                
                int ic = Columns.IndexOf(c);
                return this[ic, r];
            }
        }

        public string this[int c, string r]
        {
            get
            {
                int ir = Rows.IndexOf(r);
                return this[c, ir];
            }
        }

        public string this[int c, int r]
        {
            get
            {
                if (ValidateIndex(c, r))
                    return _data[c, r];
                else return "";
            }
            set
            {
                if (ValidateIndex(c,r))
                    _data[c,r] = value;
            }
        }

        public List<string> GetRow(int r, bool trim)
        {
            List<string> res = new List<string>(Columns.Length);
            for (int i = 0; i < Columns.Length; i++)
            {
                if (trim && string.IsNullOrEmpty(_data[i, r])) continue;
                res.Add(_data[i, r]);
            }
            return res;
        }
        public List<string> GetColumn(int c, bool trim)
        {
            List<string> res = new List<string>(Rows.Length);
            for (int i = 0; i < Rows.Length; i++)
            {
                if (trim && string.IsNullOrEmpty(_data[c, i])) continue;
                res.Add(_data[c,i]);
            }
            return res;
        }

        private bool ValidateIndex(int c, int r)
        {
            return r > 0 || r < _data.GetLength(1) && c > 0 && c < _data.GetLength(0);
        }


        public int GetColumnIndex(string key, int row = 0)
        {
            key = key.ToLower();
            int len = _data.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                if (key == _data[i, row].ToLower()) return i;
            }
            return -1;
        }

        public int GetRowIndex(string key, int col = 0)
        {
            key = key.ToLower();
            int len = _data.GetLength(1);
            for (int i = 0; i < len; i++)
            {
                if (key == _data[col, i].ToLower()) return i;
            }
            return -1;
        }

        public void Trim()
        {
            bool empty = true;
            for (int i = 0; i < _data.GetLength(0); i++)
            {
                empty = string.IsNullOrEmpty(_data[i, 0]);
                if (!empty)
                    break;
            }
            
        }        
    }
}