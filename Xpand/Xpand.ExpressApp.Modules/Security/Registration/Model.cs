using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System.Linq;
using DevExpress.ExpressApp.Security;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Registration {

    public interface IModelRegistration : IModelRegistrationActivation, IModelRegistrationEnabled {
        
        [DataSourceProperty("RoleClasses")]
        [Category("Role")]
        IModelClass RoleModelClass { get; set; }

        [CriteriaOptions("RoleModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue("[Name] = 'User' Or [Name] = 'Default'")]
        [Category("Role")]
        string RoleCriteria { get; set; }

        [DataSourceProperty("UserClasses")]
        [Category("User")]
        IModelClass UserModelClass { get; set; }

        [DataSourceProperty("UserModelClass.AllMembers")]
        [RuleRequiredField(TargetCriteria = "Enabled=true AND UserModelClass!=null")]
        [Category("User")]
        IModelMember EmailMember { get; set; }

        [Browsable(false)]
        IModelList<IModelMember> ActivationIdMembers { get; }

        [Browsable(false)]
        IModelList<IModelClass> RoleClasses { get; }

        [Browsable(false)]
        IModelList<IModelClass> UserClasses { get; }
    }

    [DomainLogic(typeof(IModelRegistration))]
    public static class ModelRegistrationDomainLogic {
        public static IModelMember Get_EmailMember(IModelRegistration modelRegistration) {
            return modelRegistration.UserModelClass?.FindMember("Email");
        }

        public static IModelClass Get_UserModelClass(IModelRegistration modelRegistration) {
            return modelRegistration.ModelClasses(typeof(ISecurityUser)).First();
        }

        public static IModelClass Get_RoleModelClass(IModelRegistration modelRegistration) {
            return modelRegistration.ModelClasses(typeof(ISecurityRole)).First();
        }

        public static IModelList<IModelClass> Get_RoleClasses(IModelRegistration modelRegistration){
            return modelRegistration.RoleModelClasses();
        }

        public static IModelList<IModelClass> Get_UserClasses(IModelRegistration modelRegistration){
            return modelRegistration.UserModelClasses();
        }

        public static IModelList<IModelClass> UserModelClasses(this IModelNode modelRegistration){
            return modelRegistration.ModelClasses(typeof (ISecurityUser));
        }

        public static IModelList<IModelClass> RoleModelClasses(this IModelNode modelRegistration){
            return modelRegistration.ModelClasses(typeof (ISecurityRole));
        }

    }
}
