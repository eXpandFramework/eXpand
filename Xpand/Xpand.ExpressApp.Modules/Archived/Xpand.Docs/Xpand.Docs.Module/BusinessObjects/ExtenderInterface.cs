using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Xpand.Docs.Module.BusinessObjects{
    public class ModelNodeAttribute : DocsBaseObject{
        private ExtenderInterface _extenderInterface;
        private string _name;
        private string _text;

        public ModelNodeAttribute(Session session) : base(session){
        }

        [Association("ExtenderInterface-Attributes")]
        public ExtenderInterface ExtenderInterface{
            get { return _extenderInterface; }
            set { SetPropertyValue("ExtenderInterface", ref _extenderInterface, value); }
        }

//        [Size(SizeAttribute.Unlimited)]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        public string Text{
            get { return _text; }
            set { SetPropertyValue("Text", ref _text, value); }
        }
    }
    [DefaultClassOptions]
    public class ExtenderInterface : ModuleArtifact{
        private ExtendedInterface _extendedInterface;


        public ExtenderInterface(Session session) : base(session){
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            Type=ModuleArtifactType.Model;
        }

        [Association("ExtendedInterface-ModelNodes")]
        public ExtendedInterface ExtendedInterface{
            get { return _extendedInterface; }
            set { SetPropertyValue("ExtendedInterface", ref _extendedInterface, value); }
        }


        [Association("ExtenderInterface-Attributes")]
        public XPCollection<ModelNodeAttribute> Attributes{
            get { return GetCollection<ModelNodeAttribute>("Attributes"); }
        }
    }

    [DefaultClassOptions]
    [DefaultProperty("Name")]
    public class ExtendedInterface : DocsBaseObject{
        private string _name;

        public ExtendedInterface(Session session) : base(session){
        }
//        [Size(SizeAttribute.Unlimited)]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("ExtendedInterface-ModelNodes")]
        public XPCollection<ExtenderInterface> ModelNodes{
            get { return GetCollection<ExtenderInterface>("ModelNodes"); }
        }
    }
}