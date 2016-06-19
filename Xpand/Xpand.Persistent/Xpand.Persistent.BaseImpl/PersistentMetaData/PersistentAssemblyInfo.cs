using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Enums;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

[assembly: Xpand.Xpo.DB.DataStore(typeof (PersistentAssemblyInfo), "WorldCreator")]

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
            _validateModelOnCompile = true;
        }

        #region IPersistentAssemblyInfo Members

        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Index(0)]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        private bool _isLegacy;
        

        [Appearance("IsLegacy_assembly", AppearanceItemType.ViewItem, "IsNewObject=false", Enabled = false)]
        public bool IsLegacy{
            get { return _isLegacy; }
            set { SetPropertyValue("IsLegacy", ref _isLegacy, value); }
        }

        [VisibleInDetailView(false)]
        [PersistentAlias("_revision")]
        [RuleValueComparison(ValueComparisonType.GreaterThan, 0)]
        public int Revision => _revision;

        public bool ValidateModelOnCompile{
            get { return _validateModelOnCompile; }
            set { SetPropertyValue("ValidateModelOnCompile", ref _validateModelOnCompile, value); }
        }

        public int Revision{
            get { return _revision; }
            set { SetPropertyValue("Revision", ref _revision, value); }
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

        IFileData IPersistentAssemblyInfo.FileData{
            get { return StrongKeyFile; }
            set { StrongKeyFile = value as StrongKeyFile; }
        }

        [Index(7)]
        [ModelDefault("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        public string CompileErrors{
            get { return _compileErrors; }
            set { SetPropertyValue("CompileErrors", ref _compileErrors, value); }
        }

        IList<IPersistentClassInfo> IPersistentAssemblyInfo.PersistentClassInfos => new ListConverter<IPersistentClassInfo, PersistentClassInfo>(PersistentClassInfos);

        #endregion
    }
}