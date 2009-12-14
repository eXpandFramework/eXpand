using System.Collections.Generic;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Enums;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class PersistentAssemblyInfo : BaseObject, IPersistentAssemblyInfo {
        CodeDomProvider _codeDomProvider;
        string _compileErrors;
        int _compileOrder;

        bool _doNotCompile;
        string _name;

        FileData _strongKeyFile;

        string _version;

        public PersistentAssemblyInfo(Session session) : base(session) {
        }

        [Index(4)]
        [FileTypeFilter("Strong Keys", 1, "*.snk")]
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public FileData StrongKeyFile {
            get { return _strongKeyFile; }
            set { SetPropertyValue("StrongKeyFile", ref _strongKeyFile, value); }
        }

        [Index(6)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode {
            get { return CodeEngine.GenerateCode(this); }
        }

        [Association("PersistentAssemblyInfo-PersistentClassInfos")]
        [Aggregated]
        public XPCollection<PersistentClassInfo> PersistentClassInfos {
            get { return GetCollection<PersistentClassInfo>("PersistentClassInfos"); }
        }

        [Association("PersistentAssemblyInfo-CodeTemplateInfos")]
        [Aggregated]
        public XPCollection<CodeTemplateInfo> CodeTemplateInfos {
            get { return GetCollection<CodeTemplateInfo>("CodeTemplateInfos"); }
        }
        #region IPersistentAssemblyInfo Members
        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Index(0)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Index(1)]
        public int CompileOrder {
            get { return _compileOrder; }
            set { SetPropertyValue("CompileOrder", ref _compileOrder, value); }
        }

        [Index(2)]
        [AllowEdit(true, AllowEditEnum.NewObject)]
        public CodeDomProvider CodeDomProvider {
            get { return _codeDomProvider; }
            set { SetPropertyValue("CodeDomProvider", ref _codeDomProvider, value); }
        }

        [Index(3)]
        public string Version {
            get { return _version; }
            set { SetPropertyValue("Version", ref _version, value); }
        }

        [Index(5)]
        public bool DoNotCompile {
            get { return _doNotCompile; }
            set { SetPropertyValue("DoNotCompile", ref _doNotCompile, value); }
        }

        IFileData IPersistentAssemblyInfo.FileData {
            get { return StrongKeyFile; }
            set { StrongKeyFile = value as FileData; }
        }

        [Index(7)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [Size(SizeAttribute.Unlimited)]
        public string CompileErrors {
            get { return _compileErrors; }
            set { SetPropertyValue("CompileErrors", ref _compileErrors, value); }
        }

        IList<IPersistentClassInfo> IPersistentAssemblyInfo.PersistentClassInfos {
            get { return new ListConverter<IPersistentClassInfo, PersistentClassInfo>(PersistentClassInfos); }
        }
        #endregion
    }
}