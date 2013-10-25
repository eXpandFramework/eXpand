using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Email.Logic {
    public interface IEmailRule : ILogicRule {
        [DataSourceProperty("ModelClass.AllMembers")]
        [Category("Email")]
        [RuleRequiredField(TargetCriteria = "EmailReceipientsContext is null")]
        IModelMember CurrentObjectEmailMember { get; set; }
    }
}