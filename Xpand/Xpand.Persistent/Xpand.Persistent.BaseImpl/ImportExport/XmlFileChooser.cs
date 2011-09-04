using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [NonPersistent]
    public class XmlFileChooser : XpandBaseCustomObject, IXmlFileChooser {
        XpandFileData _fileData;

        public XmlFileChooser(Session session)
            : base(session) {
        }

        [FileTypeFilter("Strong Keys", 1, "*.xml")]
        public XpandFileData FileData {
            get { return _fileData; }
            set { SetPropertyValue("FileData", ref _fileData, value); }
        }
        private ErrorHandling _errorHandling = ErrorHandling.CreateErrorObjects;
        [Tooltip("If false will raise an exception")]
        public ErrorHandling ErrorHandling {
            get {
                return _errorHandling;
            }
            set {
                SetPropertyValue("ErrorHandling", ref _errorHandling, value);
            }
        }
        #region IXmlFileChooser Members
        IFileData IXmlFileChooser.FileData {
            get { return _fileData; }
            set { FileData = value as XpandFileData; }
        }
        #endregion
    }
}