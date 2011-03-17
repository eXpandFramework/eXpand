using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.JobScheduler {
    public class StartSchedulerController:ViewController {
        public StartSchedulerController() {
            var simpleAction = new SimpleAction(this,"StartScheduler",PredefinedCategory.OpenObject);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            Application.FindModule<JobSchedulerModule>().Scheduler.Start();
        }
    }
    public class DisposeScheduler:WindowController {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        void FrameOnTemplateChanged(object sender, EventArgs e) {
            
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Frame.TemplateChanged-=FrameOnTemplateChanged;
                var form = Frame.Template as XtraForm;
                form.Closing+=FormOnClosing;
            }
        }

        void FormOnClosing(object sender, CancelEventArgs cancelEventArgs) {
            Application.FindModule<JobSchedulerModule>().Scheduler.Shutdown(true);
        }
    }
}
