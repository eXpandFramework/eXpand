using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System.Linq;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Registration {

    public interface IModelRegistration : IModelNode, IModelRegistrationActivation, IModelRegistrationEnabled {
        
        [DataSourceProperty("Application.BOModel")]
        IModelClass RoleModelClass { get; set; }
        
        [DataSourceProperty("Application.BOModel")]
        IModelClass UserModelClass { get; set; }

        [DataSourceProperty("UserModelClass.AllMembers")]
        [RuleRequiredField(TargetCriteria = "Enabled=true AND UserModelClass!=null")]
        IModelMember EmailMember { get; set; }

        [CriteriaOptions("RoleModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string RoleCriteria { get; set; }

        [Browsable(false)]
        IModelList<IModelMember> ActivationIdMembers { get; }
    }

    [DomainLogic(typeof(IModelRegistration))]
    public class ModelRegistrationDomainLogic {
        public static IModelList<IModelMember> Get_ActivationIdMembers(IModelRegistration modelRegistration) {
            return modelRegistration.UserModelClass == null? new CalculatedModelNodeList<IModelMember>()
                       : new CalculatedModelNodeList<IModelMember>(modelRegistration.UserModelClass.AllMembers.Where(
                           member => member.MemberInfo.MemberType == typeof (string)));
        }

        public static IModelMember Get_EmailMember(IModelRegistration modelRegistration) {
            return modelRegistration.UserModelClass!=null ? modelRegistration.UserModelClass.FindMember("Email") : null;
        }
    }
}
