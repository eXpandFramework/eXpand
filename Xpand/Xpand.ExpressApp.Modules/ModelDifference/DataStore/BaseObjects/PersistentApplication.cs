using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    public class PersistentApplication : XpandBaseCustomObject {
        string _name;
        string executableName;
        string uniqueName;

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
            get { return uniqueName; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref uniqueName, value); }
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string ExecutableName {
            get { return executableName; }

            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref executableName, value); }
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            ExecutableName = Assembly.GetEntryAssembly().ManifestModule.Name;
        }
    }
}