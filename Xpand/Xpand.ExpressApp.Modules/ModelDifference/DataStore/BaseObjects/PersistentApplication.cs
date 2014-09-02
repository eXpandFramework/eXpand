using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    public class PersistentApplication : XpandBaseCustomObject {
        string _name;
        string _executableName;
        string _uniqueName;

        public PersistentApplication(Session session) : base(session) {
        }

        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public XPCollection<ModelDifferenceObject> ModelDifferenceObjects {
            get { return GetCollection<ModelDifferenceObject>(MethodBase.GetCurrentMethod().Name.Replace("get_", "")); }
        }
        #region IPersistentApplication Members
        [DevExpress.Xpo.DisplayName("Application Name")]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Persistent]
        public string Name {
            get { return _name; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _name, value); }
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string UniqueName {
            get { return _uniqueName; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _uniqueName, value); }
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string ExecutableName {
            get { return _executableName; }

            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _executableName, value); }
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            ExecutableName = XpandModuleBase.ManifestModuleName;
        }
    }
}