using System;
using System.ComponentModel;
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
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Registration {
    public interface ILogonActionParameters : ILogonParameters {
        
    }

    public interface ILogonParameters:ICustomLogonParameter {
        void Process(XafApplication application,IObjectSpace objectSpace);
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
        [Browsable(false)]
        public object User { get; set; }

        public void Process(XafApplication application,IObjectSpace objectSpace) {
            var user = objectSpace.FindObject(XpandModuleBase.UserType, new GroupOperator(GroupOperatorType.Or,new BinaryOperator("UserName", UserName),new BinaryOperator("Email",Email)),true) as IAuthenticationStandardUser;
            if (user != null&&!objectSpace.IsNewObject(user))
                throw new UserFriendlyException(CaptionHelper.GetLocalizedText(XpandSecurityModule.XpandSecurity, "AlreadyRegistered"));

            var securityUserWithRoles = objectSpace.IsNewObject(user)? (ISecurityUserWithRoles) user
                                                               : (ISecurityUserWithRoles)objectSpace.CreateObject(XpandModuleBase.UserType);
            User = securityUserWithRoles;
            var userTypeInfo = application.TypesInfo.FindTypeInfo(XpandModuleBase.UserType);
            var modelRegistration = (IModelRegistration)((IModelOptionsRegistration)application.Model.Options).Registration;
            AddRoles(modelRegistration, userTypeInfo, securityUserWithRoles, objectSpace);

            userTypeInfo.FindMember("UserName").SetValue(securityUserWithRoles,UserName);
            userTypeInfo.FindMember("IsActive").SetValue(securityUserWithRoles,modelRegistration.ActivateUser);

            modelRegistration.EmailMember.MemberInfo.SetValue(securityUserWithRoles,Email);
            var activationLinkMember = modelRegistration.ActivationIdMember;
            if (activationLinkMember!=null) {
                activationLinkMember.MemberInfo.SetValue(securityUserWithRoles, Guid.NewGuid().ToString());
            }

            securityUserWithRoles.CallMethod("SetPassword",new []{typeof(string)}, Password);
            objectSpace.CommitChanges();
        }

        void AddRoles(IModelRegistration modelRegistration, ITypeInfo userTypeInfo, ISecurityUserWithRoles securityUserWithRoles,
                             IObjectSpace objectSpace) {
            var roles = (XPBaseCollection) userTypeInfo.FindMember("Roles").GetValue(securityUserWithRoles);
            var roleType = modelRegistration.RoleModelClass.TypeInfo.Type;
            var criteria = CriteriaOperator.Parse(modelRegistration.RoleCriteria);
            var objects = objectSpace.GetObjects(roleType, criteria);
            roles.BaseAddRange(objects);
        }
    }
    [NonPersistent]
    [ModelDefault("Caption", "Restore Password")]
    [ImageName("Action_ResetPassword")]
    public class RestorePasswordParameters : ILogonActionParameters {
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleRegularExpression(null, DefaultContexts.Save, ManageUsersOnLogonController.EmailPattern)]
        public string Email { get; set; }
        [Browsable(false)]
        public object User { get; set; }
        [Browsable(false)]
        public string Password { get; set; }

        public void Process(XafApplication application,IObjectSpace objectSpace) {
            var user = objectSpace.FindObject(XpandModuleBase.UserType, CriteriaOperator.Parse("Email = ?", Email)) as IAuthenticationStandardUser;
            if (user == null)
                throw new ArgumentException("Cannot find registered users by the provided email address!");
            User = user;
            var randomBytes = new byte[6];
            new RNGCryptoServiceProvider().GetBytes(randomBytes);
            Password = Convert.ToBase64String(randomBytes);

            user.SetPassword(Password);
            user.ChangePasswordOnFirstLogon = true;
            objectSpace.CommitChanges();
        }
    }

}