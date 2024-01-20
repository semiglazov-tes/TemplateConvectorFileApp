using Microsoft.Office.Interop.Excel;
using Range = Microsoft.Office.Interop.Excel.Range;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace TemplateWorkApp
{
    ///<summary>
    /// класс для работы с файлами Microsoft Excel
    /// </summary>
    internal class ExcelFile : AbstractFile<Workbook, Application>
    {
        private Worksheet _worksheet;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Marshal.ReleaseComObject(document);
        }

        public override void LoadFile()
        {
            app = new Application();
            document = app.Workbooks.Open(FilePath);
        }

        public override List<string> SearchPatternsInDocument()
        {
            _worksheet = (Worksheet)document.ActiveSheet;
            List<string> matchedStrings = new List<string>();
            Range usedRange = _worksheet.UsedRange;
            object[,] values = usedRange.Value;

            int rowCount = values.GetLength(0);
            int columnCount = values.GetLength(1);

            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    object cellValue = values[i, j];
                    if (cellValue != null)
                    {
                        string value = cellValue.ToString();
                        MatchCollection matches = Regex.Matches(value, "<[^<>]+>");

                        foreach (Match match in matches)
                        {
                            matchedStrings.Add(match.Value);
                        }
                    }
                }
            }
            return matchedStrings;
        }

        public override void ReplaseWordsInDocument(Dictionary<string, string> dictionaryForStringReplacement)
        {
            Range usedRange = _worksheet.UsedRange;
            object[,] values = usedRange.Value;

            int rowCount = values.GetLength(0);
            int columnCount = values.GetLength(1);

            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    object cellValue = values[i, j];
                    if (cellValue != null)
                    {
                        string value = cellValue.ToString();
                        MatchCollection matches = Regex.Matches(value, "<[^<>]+>");

                        foreach (Match match in matches)
                        {
                            string matchedString = match.Value;
                            if (dictionaryForStringReplacement.ContainsKey(matchedString))
                            {
                                value = value.Replace(matchedString, dictionaryForStringReplacement[matchedString]);
                            }
                        }
                        _worksheet.Cells[i, j] = value;
                    }
                }
            }
        }
       
        public override void SaveAndCloseFile(FileInfo incomingFile)
        {
            string filePathForSave = GeneratingASaveString(incomingFile);
            document.SaveAs(filePathForSave);
            Alert.CustomAlert("Шаблонный файл формата Excel успешно преобразован и сохранен!", System.ConsoleColor.Green);
            document.Close();
            app.Quit();
        }

        public ExcelFile(string filePath) : base(filePath)
        {
        }
    }
}



