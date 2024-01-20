using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateWorkApp
{
    ///<summary>
    /// абстрактный класс, на базе, которого будут реализовываться открытие/сохранение 
    /// файлов Microsoft World и Microsoft Excel, а также реализовано осбождение ресурсов посредством Dispose 
    /// </summary>
    abstract class AbstractFile<T,S> : IAbstractFile<T,S>
    {
        private bool _disposed;
        protected string _filePath;
        protected T document;
        protected S app;
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _disposed) return;
            _disposed = true;
        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public T Document
        {
            get { return document; }
        }
        public S App
        {
            get { return app; }
        }
        public abstract void LoadFile();
        public abstract List<string> SearchPatternsInDocument();
        public abstract void ReplaseWordsInDocument(Dictionary<string, string> dictionaryForStringReplacement);
        public abstract void SaveAndCloseFile(FileInfo incomingFile);
        public string GeneratingASaveString(FileInfo incomingFile)
        {
            string fileName;
            string filePathForSave;
            do
            {
                Console.Write("Введите имя нового файла:");
                fileName = Console.ReadLine();
                if (Validation.IsValidString(fileName))
                {
                    Alert.CustomAlert("Название файл не может быть пустой строкой. Введите название файла", ConsoleColor.Green);
                }
            }while(Validation.IsValidString(fileName));
            filePathForSave= incomingFile.DirectoryName + "\\" + fileName + incomingFile.Extension;
            return filePathForSave;
        }
        public void Dispose()
        {
            Dispose(true);
        }
        public AbstractFile(string filePath)
        {
            FilePath = filePath;
        }
        ~AbstractFile()
        {
            Dispose(false);
        }
    }
}
