using System.Collections.Generic;

namespace GameKit.Csv
{
    public class CsvReader
    {
        public readonly string[,] Data;

        private readonly string[] _fileLines;
        private int _index;
        private int _row;
        private int _col;
        private readonly List<string> _temp = new List<string>();
        private bool _canRead;

        public CsvReader(string str)
        {
            _fileLines = str.TrimEnd('\n').Replace("\r", "").Split('\n');
            var lines = new List<List<string>>(_fileLines.Length);
            _canRead = true;
            
            while (_canRead)
            {
                List<string> tmp = ReadCsvLine();
                bool empty = true;
                foreach (var t in tmp)
                {
                    empty = string.IsNullOrEmpty(t) && empty;
                }
                if (empty) continue;
                lines.Add(new List<string>(tmp));
            }

            int maxl = 0;
            foreach (var t in lines)
            {
                if (t.Count > maxl) maxl = t.Count;
            }

            Data = new string[maxl, lines.Count];

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Count; j++)
                {
                    Data[j, i] = lines[i][j];
                }
            }
        }

        private List<string> ReadCsvLine()
        {
            _temp.Clear();
            string line = "";
            bool insideQuotes = false;
            int wordStart = 0;

            while (_canRead)
            {
                if (insideQuotes)
                {
                    string s = ReadFileLine(false);
                    if (s == null) return null;
                    s = s.Replace("\\n", "\n");
                    line += "\n" + s;
                }
                else
                {
                    line = ReadFileLine(true);
                    if (line == null) return null;
                    line = line.Replace("\\n", "\n");
                    wordStart = 0;
                }

                for (int i = wordStart, imax = line.Length; i < imax; ++i)
                {
                    char ch = line[i];

                    if (ch == ',')
                    {
                        if (!insideQuotes)
                        {
                            _temp.Add(line.Substring(wordStart, i - wordStart));
                            wordStart = i + 1;
                        }
                    }
                    else if (ch == '"')
                    {
                        if (insideQuotes)
                        {
                            if (i + 1 >= imax)
                            {
                                _temp.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
                                return _temp;
                            }

                            if (line[i + 1] != '"')
                            {
                                _temp.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
                                insideQuotes = false;

                                if (line[i + 1] == ',')
                                {
                                    ++i;
                                    wordStart = i + 1;
                                }
                            }
                            else ++i;
                        }
                        else
                        {
                            wordStart = i + 1;
                            insideQuotes = true;
                        }
                    }
                }

                if (wordStart < line.Length)
                {
                    if (insideQuotes) continue;
                    _temp.Add(line.Substring(wordStart, line.Length - wordStart));
                }

                return _temp;
            }

            return null;
        }

        private string ReadFileLine(bool skipEmptyLines)
        {
            _index++;
            _canRead = _index < _fileLines.Length;
            if (skipEmptyLines && string.IsNullOrEmpty(_fileLines[_index - 1])) return ReadFileLine(true);
            return _fileLines[_index - 1];
        }
    }
}