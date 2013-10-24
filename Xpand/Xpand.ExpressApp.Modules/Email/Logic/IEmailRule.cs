using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Email.Logic {
    public interface IEmailRule : ILogicRule {
        [DataSourceProperty("ModelClass.AllMembers")]
        [Category("Email")]
        IModelMember CurrentObjectEmailMember { get; set; }
    }
}