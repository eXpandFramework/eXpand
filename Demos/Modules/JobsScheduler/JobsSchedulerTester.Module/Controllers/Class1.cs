using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.JobScheduler;
using Xpand.Persistent.Base.General;

namespace JobsSchedulerTester.Module.Controllers {
    public class Class1:ViewController {
        public Class1(){
            var simpleAction = new SimpleAction(this,"start",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            Application.FindModule<JobSchedulerModule>().Scheduler.Start();
        }
    }
}
