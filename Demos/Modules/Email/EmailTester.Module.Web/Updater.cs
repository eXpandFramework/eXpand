using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Email.BusinessObjects;

namespace EmailTester.Module.Web {
    public class Updater:ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var emailTemplate = ObjectSpace.FindObject<EmailTemplate>(null);
            if (!emailTemplate.Body.Contains("Activation")) {
                
            }
        }
    }
}
