using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;
using EditorAliases = DevExpress.ExpressApp.Editors.EditorAliases;

namespace Xpand.Docs.Module.BusinessObjects{
    public enum Platform{
        Both,
        Win,
        Web
    }

    public interface INameProvider{
        string Name { get; set; }
    }

    [NonPersistent]
    [ModelMergedDifferences(CloneViewType.ListView)]
    public abstract class DocsBaseObject : BaseObject{
        protected DocsBaseObject(Session session) : base(session){
        }
    }

    [DefaultClassOptions]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_ListEditor")]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_VIewItem")]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_Controller")]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_Permission")]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_BusinessObject")]
    [CloneView(CloneViewType.ListView, "ModuleArtifact_ListView_Action")]
    [DefaultProperty("Name")]
    public class ModuleArtifact : DocsBaseObject, INameProvider, ITreeNode{
        private XpandUser _creator;
        private string _name;
        private ModuleArtifact _parent;

        private string _text;
        private ModuleArtifactType _type;

        public ModuleArtifact(Session session) : base(session){
        }

        [InvisibleInAllViews]
        public XpandUser Creator{
            get { return _creator; }
            set { SetPropertyValue("Creator", ref _creator, value); }
        }

        [Association("ModuleChild-ModuleArtifacts")]
        public XPCollection<ModuleChild> ModuleChilds{
            get { return GetCollection<ModuleChild>("ModuleChilds"); }
        }

        public ModuleArtifactType Type{
            get { return _type; }
            set { SetPropertyValue("Type", ref _type, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        public string Text{
            get { return _text; }
            set { SetPropertyValue("Text", ref _text, value); }
        }

        [Association("ModuleArtifact-Artifacts")]
        [InvisibleInAllViews]
        public ModuleArtifact Parent{
            get { return _parent; }
            set { SetPropertyValue("Parent", ref _parent, value); }
        }

        [Association("ModuleArtifact-Artifacts")]
        public XPCollection<ModuleArtifact> Artifacts{
            get { return GetCollection<ModuleArtifact>("Artifacts"); }
        }


        [RuleRequiredField]
        [Size(255)]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        ITreeNode ITreeNode.Parent{
            get { return Parent; }
        }

        IBindingList ITreeNode.Children{
            get { return Artifacts; }
        }

        protected override void OnSaving(){
            base.OnSaving();
            if (Session.IsNewObject(this) && Creator == null && SecuritySystem.CurrentUser != null)
                Creator = Session.GetObjectByKey<XpandUser>(SecuritySystem.CurrentUserId);
        }
    }

    public enum ModuleArtifactType{
        Controller,
        ListEditor,
        ViewItem,
        Model,
        Document,
        Permission,
        BusinessObject,
        Action
    }

    [DefaultClassOptions]
    public class Module : DocsBaseObject{
        private string _installation;
        private string _name;
        private Platform _platform;

        public Module(Session session)
            : base(session){
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        public string Installation{
            get { return _installation; }
            set { SetPropertyValue("Installation", ref _installation, value); }
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("Modules-Documents")]
        public XPCollection<Document> Documents{
            get { return GetCollection<Document>("Documents"); }
        }

        [Association("Module-ModuleChilds")]
        public XPCollection<ModuleChild> ModuleChilds{
            get { return GetCollection<ModuleChild>("ModuleChilds"); }
        }


        public XPCollection<ModuleArtifact> Artifacts{
            get{
                return new XPCollection<ModuleArtifact>(Session, ModuleChilds.SelectMany(child => child.ModuleArtifacts));
            }
        }

        public Platform Platform{
            get { return _platform; }
            set { SetPropertyValue("Platform", ref _platform, value); }
        }

        public void UpdatePlatform(){
            if (ModuleChilds.Count == 1){
                Platform = ModuleChilds.First().Platform;
            }
            else if (ModuleChilds.Count == 2){
                Platform = ModuleChilds.Any(child => child.Platform == Platform.Web) ? Platform.Web : Platform.Win;
            }
            else{
                Platform = Platform.Both;
            }
        }
    }

    public class ModuleChild : DocsBaseObject{
        private string _assemblyName;
        private Module _module;
        private string _name;
        private Platform _platform;

        public ModuleChild(Session session)
            : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("ModuleChild-ModuleArtifacts")]
        public XPCollection<ModuleArtifact> ModuleArtifacts{
            get { return GetCollection<ModuleArtifact>("ModuleArtifacts"); }
        }

        [Association("Module-ModuleChilds")]
        public Module Module{
            get { return _module; }
            set { SetPropertyValue("Module", ref _module, value); }
        }

        public string AssemblyName{
            get { return _assemblyName; }
            set { SetPropertyValue("AssemblyName", ref _assemblyName, value); }
        }

        public Platform Platform{
            get { return _platform; }
            set { SetPropertyValue("Platform", ref _platform, value); }
        }

        protected override void OnSaving(){
            base.OnSaving();
            Module.UpdatePlatform();
        }
    }
}