using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    public class SerializeEmailsController:ViewController {
        public SerializeEmailsController() {
            TargetObjectType = typeof (SendEmailJobDetailDataMap);
        }
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing-=ObjectSpaceOnCommitting;
        }
        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            var sendEmailDataMap = ((SendEmailJobDetailDataMap) View.CurrentObject);
            sendEmailDataMap.Emails = CollectUsersEmails(sendEmailDataMap);
            var collectRolesEmails = CollectRolesEmails(sendEmailDataMap);
            if (!string.IsNullOrEmpty(sendEmailDataMap.Emails) && !string.IsNullOrEmpty(collectRolesEmails))
                sendEmailDataMap.Emails += ";" + collectRolesEmails;
            else if (!string.IsNullOrEmpty(collectRolesEmails)) {
                sendEmailDataMap.Emails = collectRolesEmails;
            }
        }

        string CollectRolesEmails(SendEmailJobDetailDataMap sendEmailJobDetailDataMap) {
            var baseCollection = ((XPBaseCollection)sendEmailJobDetailDataMap.GetMemberValue("Roles")).OfType<IRole>().SelectMany(role => role.Users);
            return CollectUsersEmailsCore(baseCollection); 
        }

        string CollectUsersEmails(SendEmailJobDetailDataMap sendEmailJobDetailDataMap) {
            var baseCollection = (XPBaseCollection) sendEmailJobDetailDataMap.GetMemberValue("Users");
            return CollectUsersEmailsCore(baseCollection);
        }

        string CollectUsersEmailsCore(IEnumerable baseCollection) {
            var emails = baseCollection.OfType<XPBaseObject>().Select(xpBaseObject => xpBaseObject.GetMemberValue("Email") as string).Where(value => !string.IsNullOrEmpty(value));
            return emails.Aggregate("", (current, value) => current + (value + ";")).Trim(';');
        }



    }
}
