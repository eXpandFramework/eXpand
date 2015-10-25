using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using EmailTester.Module.BusinessObjects;
using Xpand.ExpressApp.Security.Registration;

namespace EmailTester.Module.FunctionalTests{
    public class EmailObjectController : ViewController{
        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committed += ObjectSpaceOnCommitted;
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            if (View.CurrentObject is EmailTask&&!ObjectSpace.IsNewObject(View.CurrentObject)){
                ObjectSpace.Committing -= ObjectSpaceOnCommitting;
                CreateEmailObject();
            }
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            var types = new[]{typeof(Customer),typeof(Project),typeof(RegisterUserParameters),typeof(RestorePasswordParameters)};
            if (types.Contains(View.ObjectTypeInfo.Type)) {
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
                CreateEmailObject();
            }
        }

        private void CreateEmailObject(){
            var path = Path.Combine(Path.GetFullPath("."), "Emails");
            var eml = Directory.GetFiles(path, "*.eml").OrderByDescending(s => new FileInfo(s).CreationTime).First();
            var emailObject = ObjectSpace.CreateObject<EmailObject>();
            foreach (var line in File.ReadAllLines(eml)){
                var strings = line.Split(':');
                if (strings.Length == 2){
                    if (strings[0] == "From")
                        emailObject.From = strings[1].Trim();
                    else if (strings[0] == "To")
                        emailObject.To = string.Join(", ", strings[1].Trim().Split(',').Select(s => s.Trim()).OrderBy(s => s)).Trim();
                    else if (strings[0] == "Reply-To")
                        emailObject.ReplyTo = strings[1].Trim();
                    else if (strings[0] == "Subject")
                        emailObject.Subject = strings[1].Trim();
                }
                else{
                    if (line==string.Empty)
                        emailObject.Body = new Regex(@"Content-Transfer-Encoding: quoted-printable\r\n\r\n(.*)", RegexOptions.Singleline).Match(File.ReadAllText(eml)).Groups[1].Value;
                }
            }
            File.Delete(eml);
            ObjectSpace.CommitChanges();
        }
    }
}