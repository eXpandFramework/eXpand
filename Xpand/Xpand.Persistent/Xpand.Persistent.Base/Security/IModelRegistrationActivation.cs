using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Security {
    public interface IModelRegistrationActivation {
        [Category("Activation")]
        bool ActivateUser { get; set; }

        [DataSourceProperty("ActivationIdMembers")]
        [Description("For this to work you need to inherit from XpandWebApplication")]
        [Category("Activation")]
        IModelMember ActivationIdMember { get; set; }

        [RuleRequiredField(TargetCriteria = "Enabled=true AND UserModelClass!=null AND ActivationIdMember!=null")]
        [Description("For this to work you need to inherit from XpandWebApplication")]
        [Category("Activation")]
        string ActivationHost { get; set; }

        [Localizable(true)]
        [DefaultValue("<b>Activation successful!</b>")]
        string SuccessFulActivationOutput { get; set; }

        [DefaultValue("/")]
        string SuccessFulActivationReturnUrl { get; set; }
    }
}