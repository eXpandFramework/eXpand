using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class PersistentApplication : XpandBaseCustomObject {
        string _name;
        string _executableName;
        string _uniqueName;

        public PersistentApplication(Session session) : base(session) {
        }

        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public XPCollection<ModelDifferenceObject> ModelDifferenceObjects => GetCollection<ModelDifferenceObject>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));

        #region IPersistentApplication Members
        [DevExpress.Xpo.DisplayName("Application Name")]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Persistent]
        public string Name {
            get => _name;
            set => SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _name, value);
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string UniqueName {
            get => _uniqueName;
            set => SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _uniqueName, value);
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string ExecutableName {
            get => _executableName;

            set => SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _executableName, value);
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            ExecutableName = XpandModuleBase.ManifestModuleName;
        }
    }
}