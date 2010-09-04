using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    public interface IPersistentApplication {
        [DevExpress.Xpo.DisplayName("Application Name")]
        [RuleRequiredField(null, DefaultContexts.Save)]
        string Name { get; set; }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        string UniqueName { get; set; }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        string ExecutableName { get; set; }
    }
}