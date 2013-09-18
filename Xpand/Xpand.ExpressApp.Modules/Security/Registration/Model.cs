using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Security.Registration {
    [ModelAbstractClass]
    public interface IModelOptionsRegistration : IModelOptions {
        IModelRegistration Registration { get; }
    }

    public interface IModelRegistration : IModelNode {
        bool Enabled { get; set; }
        
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
    }
    [DomainLogic(typeof(IModelRegistration))]
    public class ModelRegistrationDomainLogic {
        public static IModelMember Get_EmailMember(IModelRegistration modelRegistration) {
            return modelRegistration.UserModelClass!=null ? modelRegistration.UserModelClass.FindMember("Email") : null;
        }
    }
}
