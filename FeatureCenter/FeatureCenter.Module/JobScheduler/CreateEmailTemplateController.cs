using System;
using System.IO;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;

namespace FeatureCenter.Module.JobScheduler {
    public class CreateEmailTemplateController:ViewController<DetailView> {
        public CreateEmailTemplateController() {
            TargetObjectType = typeof (SendEmailJobDataMap);
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (ObjectSpace.IsNewObject(View.CurrentObject)) {
                var sendEmailJobDataMap = ((SendEmailJobDataMap) View.CurrentObject);
                var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), "SendWelcomeMail.html.cshtml");
                if (manifestResourceStream == null) throw new NotImplementedException();
                sendEmailJobDataMap.EmailTemplate = new StreamReader(manifestResourceStream).ReadToEnd();
            }
        }
    }
}
