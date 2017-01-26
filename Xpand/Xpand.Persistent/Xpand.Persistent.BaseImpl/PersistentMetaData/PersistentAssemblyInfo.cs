using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Enums;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData{
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof (IPersistentAssemblyInfo))]
    [DefaultProperty(nameof(Name))]
    public class PersistentAssemblyInfo : XpandBaseCustomObject, IPersistentAssemblyInfo{
        private CodeDomProvider _codeDomProvider;
        private string _errors;
        private int _compileOrder;

        private bool _doNotCompile;
        private string _name;
        [Persistent("Revision")]
        private int _revision;

        private StrongKeyFile _strongKeyFile;

        public PersistentAssemblyInfo(Session session)
            : base(session){
        }

        [Index(4)]
        [FileTypeFilter("Strong Keys", 1, "*.snk")]
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public StrongKeyFile StrongKeyFile{
            get { return _strongKeyFile; }
            set { SetPropertyValue("StrongKeyFile", ref _strongKeyFile, value); }
        }

        [Index(6)]
        [ModelDefault("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.CSCodePropertyEditor)]
        public string GeneratedCode => this.GenerateCode();

        [Association("PersistentAssemblyInfo-PersistentClassInfos")]
        [Aggregated]
        public XPCollection<PersistentClassInfo> PersistentClassInfos => GetCollection<PersistentClassInfo>("PersistentClassInfos");


        public override void AfterConstruction(){
            base.AfterConstruction();
            Attributes.Add(new PersistentAssemblyVersionAttributeInfo(Session));
        }

        #region IPersistentAssemblyInfo Members

        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Index(0)]
//        [RuleValidFileName]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
        

        [VisibleInDetailView(false)]
        [PersistentAlias("_revision")]
        [RuleValueComparison(ValueComparisonType.GreaterThan, 0)]
        public int Revision => _revision;

        int IPersistentAssemblyInfo.Revision{
            get { return _revision; }
            set{
                _revision = value;
                OnChanged(nameof(_revision));
            }
        }


        [Index(1)]
        public int CompileOrder{
            get { return _compileOrder; }
            set { SetPropertyValue("CompileOrder", ref _compileOrder, value); }
        }

        [Association("PersistentAssemblyInfo-Attributes")]
        public XPCollection<PersistentAssemblyAttributeInfo> Attributes => GetCollection<PersistentAssemblyAttributeInfo>("Attributes");

        IList<IPersistentAssemblyAttributeInfo> IPersistentAssemblyInfo.Attributes => new ListConverter<IPersistentAssemblyAttributeInfo, PersistentAssemblyAttributeInfo>(Attributes);

        [Index(2)]
        [AllowEdit(true, AllowEditEnum.NewObject)]
        public CodeDomProvider CodeDomProvider{
            get { return _codeDomProvider; }
            set { SetPropertyValue("CodeDomProvider", ref _codeDomProvider, value); }
        }


        [Index(5)]
        public bool DoNotCompile{
            get { return _doNotCompile; }
            set { SetPropertyValue("DoNotCompile", ref _doNotCompile, value); }
        }

        IFileData IPersistentAssemblyInfo.StrongKeyFileData{
            get { return StrongKeyFile; }
            set { StrongKeyFile = value as StrongKeyFile; }
        }

        [Index(7)]
        [ModelDefault("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        public string Errors{
            get { return _errors; }
            set { SetPropertyValue("Errors", ref _errors, value); }
        }

        IList<IPersistentClassInfo> IPersistentAssemblyInfo.PersistentClassInfos => new ListConverter<IPersistentClassInfo, PersistentClassInfo>(PersistentClassInfos);

        #endregion
    }
}