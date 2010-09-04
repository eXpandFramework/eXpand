using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [NonPersistent]
    public class XmlFileChooser : BaseObject, IXmlFileChooser {
        FileData _fileData;

        public XmlFileChooser(Session session)
            : base(session) {
        }

        [FileTypeFilter("Strong Keys", 1, "*.xml")]
        public FileData FileData {
            get { return _fileData; }
            set { SetPropertyValue("FileData", ref _fileData, value); }
        }
        #region IXmlFileChooser Members
        IFileData IXmlFileChooser.FileData {
            get { return _fileData; }
            set { FileData = value as FileData; }
        }
        #endregion
    }
}