using DevExpress.ExpressApp;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;

namespace FeatureCenter.Module.JobScheduler {
    public class CreateEmailTemplateController : ViewController<DetailView> {
        public CreateEmailTemplateController() {
            TargetObjectType = typeof(SendEmailJobDataMap);
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (ObjectSpace.IsNewObject(View.CurrentObject)) {
                var sendEmailJobDataMap = ((SendEmailJobDataMap)View.CurrentObject);
                sendEmailJobDataMap.EmailTemplate = "<P>Criteria @Model.Criteria have been evaluated against @Model.DataType and found @Model.Count objects.</P><P>Severity:@Model.Severity</P>";
            }
        }
    }
}
