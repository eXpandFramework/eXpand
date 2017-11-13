using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.Persistent.BaseImpl;

namespace MainDemo.Module.BusinessObjects {
    [ImageName("BO_FileAttachment")]
    public class PortfolioFileData : FileAttachmentBase {
        public PortfolioFileData(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            documentType = DocumentType.Unknown;
        }
        protected Resume resume;
        [Association("Resume-PortfolioFileData")]
        public Resume Resume {
            get {
                return resume;
            }
            set {
                SetPropertyValue<Resume>("Resume", ref resume, value);
            }
        }
        private DocumentType documentType;
        public DocumentType DocumentType {
            get {
                return documentType;
            }
            set {
                SetPropertyValue<DocumentType>("DocumentType", ref documentType, value);
            }
        }
    }
    public enum DocumentType {
        SourceCode = 1,
        Tests = 2,
        Documentation = 3,
        Diagrams = 4,
        ScreenShots = 5,
        Unknown = 6
    };
}
