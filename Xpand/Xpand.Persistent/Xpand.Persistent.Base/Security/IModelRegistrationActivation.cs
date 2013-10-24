using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Security {
    public interface IModelRegistrationActivation {
        bool ActivateUser { get; set; }

        [DataSourceProperty("ActivationIdMembers")]
        [Description("For this to work you need to inherit from XpandWebApplication")]
        IModelMember ActivationIdMember { get; set; }

        [RuleRequiredField(TargetCriteria = "Enabled=true AND UserModelClass!=null AND ActivationIdMember!=null")]
        [Description("For this to work you need to inherit from XpandWebApplication")]
        string ActivationHost { get; set; }
    }
}