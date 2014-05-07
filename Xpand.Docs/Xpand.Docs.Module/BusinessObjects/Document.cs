using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.Docs.Module.BusinessObjects{

    [DefaultClassOptions]
    public class Document : ModuleArtifact{
        private string _author;

        private string _url;

        public Document(Session session) : base(session){
        }

        [Persistent("AuthorName")]
        public string Author{
            get { return _author; }
            set { SetPropertyValue("Author", ref _author, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HyperLinkPropertyEditor)]
        public string Url{
            get { return _url; }
            set { SetPropertyValue("Url", ref _url, value); }
        }

        [Association("Modules-Documents")]
        public XPCollection<Module> Modules{
            get { return GetCollection<Module>("Modules"); }
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            Type = ModuleArtifactType.Document;
        }
    }
}