using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using EmailTester.Module.BusinessObjects;
using Xpand.ExpressApp.Email.BusinessObjects;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.Security;

namespace EmailTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole)) {
                var emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Subject = "User activation";
                emailTemplate.Body = string.Format("A new user @Model.User.UserName has been created. To activate the account please click the following link {0}@Model.User.Activation",
                                                   ((IModelRegistration)
                                                    ((IModelOptionsRegistration) CaptionHelper.ApplicationModel.Options).Registration).ActivationHost+"?ua=");
                
                emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Subject = "pass forgotten";
                emailTemplate.Body = "We created a temporary password (@Model.Password) for the UserName (@Model.User.UserName). Please login to reset it";
                
                emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Subject = "New Customer";
                emailTemplate.Body = "A new customer created with fullname @Model.FullName";

                emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Subject = "Project created";
                emailTemplate.Body = "We created a new project (@Model.Name).";
                
                emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Subject = "TaskStatusChanged";
                emailTemplate.Body = "The Status of (@Model.Subject) changed to (@Model.Status)";
            }
            var adminRole = ObjectSpace.GetAdminRole("Admin");
            var adminUser = (User) adminRole.GetUser("Admin");
            adminUser.Email = "apostolis.bekiaris@gmail.com";

            var userRole = ObjectSpace.GetRole("User");
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            ObjectSpace.CommitChanges();
        }
    }
}
