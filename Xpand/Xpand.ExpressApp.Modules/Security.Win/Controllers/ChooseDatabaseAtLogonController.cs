using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Extensions.AppDomainExtensions;

namespace Xpand.ExpressApp.Security.Win.Controllers {
    public class ChooseDatabaseAtLogonController: WindowController {
        private bool _isLogOff;

        public ChooseDatabaseAtLogonController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelOptionsChooseDatabaseAtLogon)Application.Model.Options).ChooseDatabaseAtLogon){
                Application.LoggedOff+=ApplicationOnLoggedOff;
                Frame.GetController<LogoffController>().Actions.First().Executing+=OnExecuting;
            }
        }

        private void OnExecuting(object sender, CancelEventArgs cancelEventArgs){
            _isLogOff = !cancelEventArgs.Cancel;
        }

        private void ApplicationOnLoggedOff(object sender, EventArgs eventArgs){
            if (!_isLogOff)
                return;
            var xafApplication = ((XafApplication) sender);
            xafApplication.LoggedOff-=ApplicationOnLoggedOff;
            var info = new ProcessStartInfo();
            var exeName = Path.Combine(AppDomain.CurrentDomain.ApplicationPath(), AppDomain.CurrentDomain.ApplicationName());
            info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + exeName + "\"";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
            xafApplication.Exit();
        }

    }
}
