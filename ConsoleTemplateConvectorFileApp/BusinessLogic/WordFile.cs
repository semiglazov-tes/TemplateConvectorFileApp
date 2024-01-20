
using Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TemplateWorkApp;
using Range = Microsoft.Office.Interop.Word.Range;

namespace TemplatesWorkApp
{
    ///<summary>
    /// класс для работы с файлами Microsoft World
    /// </summary>
    internal class WordFile : AbstractFile<Document, Application>
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Marshal.ReleaseComObject(document);
        }

        public override void LoadFile()
        {
            app = new Application();
            document = app.Documents.Open(FilePath);
        }

        public override List<string> SearchPatternsInDocument()
        {
            List<string> matchedStrings = new List<string>();
            foreach (Range word in document.StoryRanges)
            {
                var matches = Regex.Matches(word.Text, "<[^<>]+>");
                foreach (Match match in matches)
                {
                    matchedStrings.Add(match.Value);
                }
            }
            return matchedStrings;
        }

        public override void ReplaseWordsInDocument(Dictionary<string, string> dictionaryForStringReplacement)
        {
            foreach (Range word in document.StoryRanges)
            {
                var matches = Regex.Matches(word.Text, "<[^<>]+>");
                foreach (Match match in matches)
                {
                    var replacement = dictionaryForStringReplacement[match.Value];
                    if (replacement != null)
                    {
                        word.Find.Execute(match.Value, Replace: WdReplace.wdReplaceAll, ReplaceWith: replacement);
                    }
                }
            }
        }

        public override void SaveAndCloseFile(FileInfo incomingFile)
        {
            string filePathForSave = GeneratingASaveString(incomingFile);
            document.SaveAs2(filePathForSave);
            Alert.CustomAlert("Шаблонный файл формата Excel успешно преобразован и сохранен!", System.ConsoleColor.Green);
            document.Close();
            app.Quit();
        }

        public WordFile(string filePath) : base(filePath)
        {
        }
    }
}
