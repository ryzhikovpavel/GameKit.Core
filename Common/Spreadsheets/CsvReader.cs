using System.Collections.Generic;

#pragma warning disable 169

namespace GameKit
{
    public class CsvReader
    {
        public readonly string[,] data;
        public List<List<string>> lines;

        private string[] fileLines;
        private int index;
        private int row;
        private int col;
        private List<string> temp = new List<string>();
        private bool canRead;

        public CsvReader(string str)
        {
            fileLines = str.TrimEnd('\n').Replace("\r", "").Split('\n');
            lines = new List<List<string>>(fileLines.Length);
            canRead = true;
            
            while (canRead)
            {
                List<string> tmp = ReadCSVLine();
                bool empty = true;
                for (int i = 0; i < tmp.Count; i++)
                {
                    empty = string.IsNullOrEmpty(tmp[i]) && empty;
                }
                if (empty) continue;
                lines.Add(new List<string>(tmp));
            }

            int maxl = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Count > maxl) maxl = lines[i].Count;
            }

            data = new string[maxl, lines.Count];

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Count; j++)
                {
                    data[j, i] = lines[i][j];
                }
            }
        }

        private List<string> ReadCSVLine()
        {
            temp.Clear();
            string line = "";
            bool insideQuotes = false;
            int wordStart = 0;

            while (canRead)
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
                            temp.Add(line.Substring(wordStart, i - wordStart));
                            wordStart = i + 1;
                        }
                    }
                    else if (ch == '"')
                    {
                        if (insideQuotes)
                        {
                            if (i + 1 >= imax)
                            {
                                temp.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
                                return temp;
                            }

                            if (line[i + 1] != '"')
                            {
                                temp.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
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
                    temp.Add(line.Substring(wordStart, line.Length - wordStart));
                }

                return temp;
            }

            return null;
        }

        private string ReadFileLine(bool skipEmptyLines)
        {
            index++;
            canRead = index < fileLines.Length;
            if (skipEmptyLines && string.IsNullOrEmpty(fileLines[index - 1])) return ReadFileLine(true);
            return fileLines[index - 1];
        }
    }
}