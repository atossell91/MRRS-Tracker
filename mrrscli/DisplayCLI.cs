using System;
using System.Collections.Generic;
using System.Text;

public class DisplayCLI {
    public class Column {
        public enum ColumnJustification {
            Left,
            Right,
            Centre
        }
        public int Width { get; set; } = -1;
        public string LeftBorder { get; set; } = String.Empty;
        public string RightBorder { get; set;} = String.Empty;
        public char BlankChar = ' ';
        public ColumnJustification Justification { get; set; }

        public static string RepeatStr(char str, int count) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < count; ++i) {
                builder.Append(str);
            }
            return builder.ToString();
        }

        public string Format(string str) {
            
            if (String.IsNullOrEmpty(str)) {
                return "";
            }

            if (Width < 0) {
                return str;
            }

            int lenDiff = Width - str.Length;
            if (lenDiff < 0) {
                return str.Substring(0, Width);
            }
            else {
                if (Justification == ColumnJustification.Left) {
                    string spaces = RepeatStr(BlankChar, lenDiff);
                    return LeftBorder + str + spaces + RightBorder;

                }
                else if (Justification == ColumnJustification.Right) {
                    string spaces = RepeatStr(BlankChar, lenDiff);
                    return LeftBorder + spaces + str + RightBorder;
                }
                else {
                    int leftSpaceCount = (int)((double)lenDiff/2);
                    int rightSpaceCount = (int)((((double)lenDiff)/2)+0.999999);
                    string leftPad = RepeatStr(BlankChar, leftSpaceCount);
                    string rightPad = RepeatStr(BlankChar, rightSpaceCount);
                    return LeftBorder + leftPad + str + rightPad + RightBorder;
                }
            }
        }
    }


    private List<Column> columns = new List<Column>();

    public void AddColumn(Column c) {
        columns.Add(c);
    }

    public void ClearColumns() {
        columns.Clear();
    }

    public void PrintDivider(string str) {
        Column c = new Column() {
            Width = 80,
            Justification = Column.ColumnJustification.Centre,
            BlankChar = '-'
        };

        if (str == String.Empty) {
            Console.WriteLine(Column.RepeatStr('-', 80));            
        }
        else {
            Console.WriteLine(c.Format(" " + str + " "));
        }
    }

    public void PrintRow(int indentCount, params string[] data) {
        string indent = Column.RepeatStr(' ', indentCount);
        for (int i = 0; i < data.Length; ++i) {
            string s = String.Empty;
            if (i < columns.Count) {
                s = columns[i].Format(data[i]);
            }
            else {
                s = " " + data[i];
            }
            Console.Write(indent + s);
        }
        Console.WriteLine("");
        
    }

    public void PrintRow(params string[] data) {
        PrintRow(0, data);
    }
}