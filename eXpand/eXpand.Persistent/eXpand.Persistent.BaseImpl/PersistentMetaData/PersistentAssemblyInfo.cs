using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class PersistentAssemblyInfo:BaseObject, IPersistentAssemblyInfo {
        public PersistentAssemblyInfo(Session session) : base(session) {
        }
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode
        {
            get {
                return PersistentClassInfos.Aggregate<PersistentClassInfo, string>(null, (current, persistentClassInfo) => current + persistentClassInfo.GeneratedCode);
            }
        }
        
        private string _name;
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
        [Association("PersistentAssemblyInfo-PersistentClassInfos")][Aggregated]
        public XPCollection<PersistentClassInfo> PersistentClassInfos
        {
            get
            {
                return GetCollection<PersistentClassInfo>("PersistentClassInfos");
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