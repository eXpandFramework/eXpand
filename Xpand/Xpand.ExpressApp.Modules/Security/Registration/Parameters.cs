using System;
using System.Security.Cryptography;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Registration {
    public interface ILogonActionParameters {
        void Process(IObjectSpace objectSpace);
    }
    public interface ILogonRegistrationParameters {
        void Process(IObjectSpace objectSpace);
    }

    [NonPersistent]
    [ModelDefault("Caption", "Register User")]
    [ImageName("BO_User")]
    public class RegisterUserParameters : ILogonRegistrationParameters {
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string UserName { get; set; }
        public string Password { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        [RuleRegularExpression(null, DefaultContexts.Save, ManageUsersOnLogonController.EmailPattern)]
        public string Email { get; set; }
        public void Process(IObjectSpace objectSpace) {
            var user = objectSpace.FindObject(XpandModuleBase.UserType, new GroupOperator(GroupOperatorType.Or,new BinaryOperator("UserName", UserName),new BinaryOperator("Email",Email))) as IAuthenticationStandardUser;
            if (user != null)
                throw new ArgumentException("The login with the entered UserName or Email was already registered within the system");
            else
//                SecurityExtensionsModule.CreateSecuritySystemUser(objectSpace, UserName, Email, Password, false);
                throw new UserFriendlyException("A new user has successfully been registered");
        }

    }
    [NonPersistent]
    [ModelDefault("Caption", "Restore Password")]
    [ImageName("Action_ResetPassword")]
    public class RestorePasswordParameters : ILogonActionParameters {
        public const string ValidationContext = "RestorePasswordContext";
        [RuleRequiredField(null, ValidationContext)]
        [RuleRegularExpression(null, ValidationContext, ManageUsersOnLogonController.EmailPattern)]
        public string Email { get; set; }
        public void Process(IObjectSpace objectSpace) {
            if (string.IsNullOrEmpty(Email))
                throw new ArgumentException("Email address is not specified!");
            var user = objectSpace.FindObject(XpandModuleBase.UserType, CriteriaOperator.Parse("Email = ?", Email)) as IAuthenticationStandardUser;
            if (user == null)
                throw new ArgumentException("Cannot find registered users by the provided email address!");
            var randomBytes = new byte[6];
            new RNGCryptoServiceProvider().GetBytes(randomBytes);
            string password = Convert.ToBase64String(randomBytes);
            //Dennis: Resets the old password and generates a new one.
            user.SetPassword(password);
            user.ChangePasswordOnFirstLogon = true;
            objectSpace.CommitChanges();
            EmailLoginInformation(Email, password);
        }
        public static void EmailLoginInformation(string email, string password) {
            //Dennis: Send an email with the login details here. 
            //Refer to http://msdn.microsoft.com/en-us/library/system.net.mail.mailmessage.aspx for more details.
            //throw new UserFriendlyException("Password recovery link was sent to " + email);
        }
    }

}