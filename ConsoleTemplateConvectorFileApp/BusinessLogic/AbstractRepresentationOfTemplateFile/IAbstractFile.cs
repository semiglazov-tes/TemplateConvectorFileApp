using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateWorkApp
{
    public interface IAbstractFile<out T, out S> : IDisposable
    {
        T Document { get; }
        S App { get; }
        void LoadFile();
        void SaveAndCloseFile(FileInfo incomingFile);
        List<string> SearchPatternsInDocument();
        void ReplaseWordsInDocument(Dictionary<string, string> dictionaryForStringReplacement);
    }
}

