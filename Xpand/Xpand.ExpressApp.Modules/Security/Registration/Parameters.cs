using System;
using System.Security.Cryptography;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Fasterflect;

namespace Xpand.ExpressApp.Security.Registration {
    public interface ILogonActionParameters : ILogonParameters {
        
    }

    public interface ILogonParameters {
        void Process(XafApplication objectSpace);
    }

    public interface ILogonRegistrationParameters : ILogonParameters {
    }

    [NonPersistent]
    [ModelDefault("Caption", "Register User")]
    [ImageName("BO_User")]
    public class RegisterUserParameters : ILogonRegistrationParameters {
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string UserName { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        [ModelDefault("IsPassword", "true")]
        public string Password { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleRegularExpression(null, DefaultContexts.Save, ManageUsersOnLogonController.EmailPattern)]
        public string Email { get; set; }
        public void Process(XafApplication application) {
            var objectSpace = application.CreateObjectSpace();
            var user = objectSpace.FindObject(XpandModuleBase.UserType, new GroupOperator(GroupOperatorType.Or,new BinaryOperator("UserName", UserName),new BinaryOperator("Email",Email))) as IAuthenticationStandardUser;
            if (user != null)
                throw new ArgumentException(CaptionHelper.GetLocalizedText(XpandSecurityModule.XpandSecurity, "AlreadyRegistered"));

            var securityUserWithRoles = (ISecurityUserWithRoles)objectSpace.CreateObject(XpandModuleBase.UserType);
            var userTypeInfo = application.TypesInfo.FindTypeInfo(XpandModuleBase.UserType);
            var modelRegistration = ((IModelOptionsRegistration)application.Model.Options).Registration;
            AddRoles(modelRegistration, userTypeInfo, securityUserWithRoles, objectSpace);

            userTypeInfo.FindMember("UserName").SetValue(securityUserWithRoles,UserName);
            modelRegistration.EmailMember.MemberInfo.SetValue(securityUserWithRoles,Email);
            userTypeInfo.CallMethod("SetPassword", Password);

            objectSpace.CommitChanges();
        }

        void AddRoles(IModelRegistration modelRegistration, ITypeInfo userTypeInfo, ISecurityUserWithRoles securityUserWithRoles,
                             IObjectSpace objectSpace) {
            var roles = (XPBaseCollection) userTypeInfo.FindMember("Roles").GetValue(securityUserWithRoles);
            var roleType = modelRegistration.RoleModelClass.TypeInfo.Type;
            roles.BaseAddRange(objectSpace.GetObjects(roleType, modelRegistration.RoleCriteria));
        }
    }
    [NonPersistent]
    [ModelDefault("Caption", "Restore Password")]
    [ImageName("Action_ResetPassword")]
    public class RestorePasswordParameters : ILogonActionParameters {
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleRegularExpression(null, DefaultContexts.Save, ManageUsersOnLogonController.EmailPattern)]
        public string Email { get; set; }
        public void Process(XafApplication application) {
            if (string.IsNullOrEmpty(Email))
                throw new ArgumentException("Email address is not specified!");
            var objectSpace = application.CreateObjectSpace();
            var user = objectSpace.FindObject(XpandModuleBase.UserType, CriteriaOperator.Parse("Email = ?", Email)) as IAuthenticationStandardUser;
            if (user == null)
                throw new ArgumentException("Cannot find registered users by the provided email address!");
            var randomBytes = new byte[6];
            new RNGCryptoServiceProvider().GetBytes(randomBytes);
            string password = Convert.ToBase64String(randomBytes);
            
            user.SetPassword(password);
            user.ChangePasswordOnFirstLogon = true;
            objectSpace.CommitChanges();
        }
    }

}