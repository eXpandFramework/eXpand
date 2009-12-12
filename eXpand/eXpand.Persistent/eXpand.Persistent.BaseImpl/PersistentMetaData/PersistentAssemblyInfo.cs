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
    public class PersistentAssemblyInfo:BaseObject, IPersistentAssemblyInfo {
        public PersistentAssemblyInfo(Session session) : base(session) {
        }
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode
        {
            get { return CodeEngine.GenerateCode(this); }
        }
        
        private string _name;
        [RuleRequiredField(null,DefaultContexts.Save)]
        [RuleUniqueValue(null,DefaultContexts.Save)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private bool _doNotCompile;
        public bool DoNotCompile
        {
            get
            {
                return _doNotCompile;
            }
            set
            {
                SetPropertyValue("DoNotCompile", ref _doNotCompile, value);
            }
        }

        private CodeDomProvider _codeDomProvider;
        [AllowEdit(true,AllowEditEnum.NewObject)]
        public CodeDomProvider CodeDomProvider
        {
            get
            {
                return _codeDomProvider;
            }
            set
            {
                SetPropertyValue("CodeDomProvider", ref _codeDomProvider, value);
            }
        }
        private FileData _strongKeyFile;
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public FileData StrongKeyFile
        {
            get
            {
                return _strongKeyFile;
            }
            set
            {
                SetPropertyValue("StrongKeyFile", ref _strongKeyFile, value);
            }
        }
        IFileData IPersistentAssemblyInfo.FileData {
            get { return StrongKeyFile; }
            set { StrongKeyFile=value as FileData; }
        }

        [Association("PersistentAssemblyInfo-PersistentClassInfos")][Aggregated]
        public XPCollection<PersistentClassInfo> PersistentClassInfos
        {
            get
            {
                return GetCollection<PersistentClassInfo>("PersistentClassInfos");
            }
        }
        [Association("PersistentAssemblyInfo-CodeTemplateInfos")][Aggregated]
        public XPCollection<CodeTemplateInfo> CodeTemplateInfos
        {
            get
            {
                return GetCollection<CodeTemplateInfo>("CodeTemplateInfos");
            }
        }
        private string _compileErrors;
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        [Size(SizeAttribute.Unlimited)]
        public string CompileErrors
        {
            get
            {
                return _compileErrors;
            }
            set
            {
                SetPropertyValue("CompileErrors", ref _compileErrors, value);
            }
        }
        IList<IPersistentClassInfo> IPersistentAssemblyInfo.PersistentClassInfos
        {
            get { return new ListConverter<IPersistentClassInfo,PersistentClassInfo>(PersistentClassInfos); }
        }
    }
}