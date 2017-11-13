using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.Persistent.BaseImpl;

namespace MainDemo.Module.BusinessObjects {
    [FileAttachment("File")]
    [DefaultClassOptions, ImageName("BO_Resume")]
    public class Resume : BaseObject {
        private Contact contact;
        private FileData file;
        public Resume(Session session)
            : base(session) {
        }
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public FileData File {
            get {
                return file;
            }
            set {
                SetPropertyValue("File", ref file, value);
            }
        }
        public Contact Contact {
            get {
                return contact;
            }
            set {
                SetPropertyValue("Contact", ref contact, value);
            }
        }
        [Aggregated, Association("Resume-PortfolioFileData")]
        public XPCollection<PortfolioFileData> Portfolio {
            get {
                return GetCollection<PortfolioFileData>("Portfolio");
            }
        }
    }
}
