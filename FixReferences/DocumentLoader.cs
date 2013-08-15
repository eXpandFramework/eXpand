using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace FixReferences {
    public interface IDocumentHelper {
        XDocument GetXDocument(string file);
        void Save(XDocument document, string file);
        IList<string> SavedFiles { get; }
    }

    public class DocumentHelper : IDocumentHelper {
        private readonly IList<string> _savedFiles=new List<string>();

        public XDocument GetXDocument(string file) {
            Environment.CurrentDirectory = Path.GetDirectoryName(file) + "";
            XDocument document;
            using (var fileStream = File.OpenRead(file)) {
                document = XDocument.Load(fileStream);
            }
            return document;
        }

        // Fields...

        public IList<string> SavedFiles {
            get { return _savedFiles; }
        }
        
        public void Save(XDocument document, string file) {
            document.Save(file, SaveOptions.None);
            _savedFiles.Add(file);
        }

    }
}