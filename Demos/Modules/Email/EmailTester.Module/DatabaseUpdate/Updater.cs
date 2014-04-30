using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Email.BusinessObjects;
using Xpand.ExpressApp.Security.Core;

namespace EmailTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole)) {
                
                var emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Configure(EmailTemplateConfig.UserActivation, "http://localhost:50822/");;
                
                emailTemplate = ObjectSpace.CreateObject<EmailTemplate>();
                emailTemplate.Configure(EmailTemplateConfig.PassForgotten);
                
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
            var adminUser = (XpandUser) adminRole.GetUser("Admin");
            adminUser.Email = "apostolis.bekiaris@gmail.com";

            var userRole = ObjectSpace.GetRole("User");
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            ObjectSpace.CommitChanges();
        }
    }
}
