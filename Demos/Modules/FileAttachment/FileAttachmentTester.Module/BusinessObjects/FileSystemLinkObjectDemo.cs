using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.FileAttachment.BusinessObjects;

namespace FileAttachmentTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [FileAttachment("File")]
    public class FileSystemLinkObjectDemo : BaseObject {
        public FileSystemLinkObjectDemo(Session session) : base(session) { }
        
        public FileSystemLinkObject File {
            get { return GetPropertyValue<FileSystemLinkObject>("File"); }
            set { SetPropertyValue("File", value); }
        }
    }
}
