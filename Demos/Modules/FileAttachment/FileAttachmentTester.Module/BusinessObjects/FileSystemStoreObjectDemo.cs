using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.FileAttachment.BusinessObjects;

namespace FileAttachmentTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [FileAttachment("File")]
    public class FileSystemStoreObjectDemo : BaseObject {
        public FileSystemStoreObjectDemo(Session session) : base(session) { }
        
        public FileSystemStoreObject File {
            get { return GetPropertyValue<FileSystemStoreObject>("File"); }
            set { SetPropertyValue("File", value); }
        }
    }
}
