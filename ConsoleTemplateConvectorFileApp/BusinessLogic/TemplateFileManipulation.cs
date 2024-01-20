using System.Text.RegularExpressions;
using System;
using TemplatesWorkApp;
using System.Collections.Generic;
using System.IO;

namespace TemplateWorkApp
{
    ///<summary>
    /// Создание файла ---> поиск/замена шаблонных строк ---> сохранение файла
    /// </summary>
    internal class TemplateFileManipulation
    {
        private FileInfo _incomingFile;
        private IAbstractFile<object, object> _templateFile;

        private List<string> _BarTemplate = new List<string> { };

        public Dictionary<string, string> _dictionaryForStringReplacement = new Dictionary<string, string> { };

        private void _creatingFileInstance()
        {
            var log = new Loger(DateTime.Now, $"Создание экземпляра- {Path.GetFileNameWithoutExtension(_incomingFile.Name)} в программе"); 
            string pattern = @"^.x[a-zA-Z]+$";
            switch (Regex.IsMatch(_incomingFile.Extension, pattern))
            {
                case false:
                    _templateFile = new WordFile(_incomingFile.FullName);
                    break;

                case true:
                    _templateFile = new ExcelFile(_incomingFile.FullName);
                    break;
            }
            log.FinishLog(DateTime.Now);
        }

        private void _getReplacementStringsFromUser()
        {

            foreach (string barTemplate in _BarTemplate)
            {
                Console.WriteLine($"Введите замену для строки {barTemplate}:");
                string replacement = Console.ReadLine();
                while (String.IsNullOrEmpty(replacement) == true)
                {
                    Alert.CustomAlert("Строка для замены не может быть пустой строкой", ConsoleColor.Red);
                    Console.WriteLine($"Введите замену для строки {barTemplate}:");
                    replacement = Console.ReadLine();
                }
                _dictionaryForStringReplacement[barTemplate] = replacement;
            }
            Console.Clear();
        }


        public void StartManipulatingTemplateFileData(FileInfo templatesFile)
        {
            _incomingFile = templatesFile;
            _creatingFileInstance();
            using (_templateFile)
            {
                _templateFile.LoadFile();

                _BarTemplate = _templateFile.SearchPatternsInDocument();

                if(Validation.BarTemplateListValid(_BarTemplate)==false)
                {
                    Alert.CustomAlert("В файле шаблона остуствуют шаблонные строки для замены",ConsoleColor.Red);
                    return;
                }

                _getReplacementStringsFromUser();

                _templateFile.ReplaseWordsInDocument(_dictionaryForStringReplacement);

                _templateFile.SaveAndCloseFile(_incomingFile);
            }

        }
        public TemplateFileManipulation()
        {

        }
    }
}
