using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.Persistent.BaseImpl.ImportExport {
    [NonPersistent]
    public class XmlFileChooser : BaseObject, IXmlFileChooser {
        public XmlFileChooser(Session session)
            : base(session)
        {
        }

        IFileData IXmlFileChooser.FileData {
            get { return _fileData; }
            set { FileData=value as FileData      ; }
        }
        private FileData _fileData;
        public FileData FileData
        {
            get
            {
                return _fileData;
            }
            set
            {
                SetPropertyValue("FileData", ref _fileData, value);
            }
        }
    }
}