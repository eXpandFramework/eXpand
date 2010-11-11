using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [NonPersistent]
    public class XmlFileChooser : XpandCustomObject, IXmlFileChooser {
        XpandFileData _fileData;

        public XmlFileChooser(Session session)
            : base(session) {
        }

        [FileTypeFilter("Strong Keys", 1, "*.xml")]
        public XpandFileData FileData {
            get { return _fileData; }
            set { SetPropertyValue("FileData", ref _fileData, value); }
        }
        #region IXmlFileChooser Members
        IFileData IXmlFileChooser.FileData {
            get { return _fileData; }
            set { FileData = value as XpandFileData; }
        }
        #endregion
    }
}