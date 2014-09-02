using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Win.Reminders {
    public class ReminderAlertController : Scheduler.Reminders.ReminderAlertController {
        RemindersForm _remindersForm;

        protected override void OnActivated(){
            base.OnActivated();
            Frame.TemplateChanged += FrameOnTemplateChanged;    
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.TemplateChanged-=FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            if (((IModelOptionsWin) Application.Model.Options).UIType == UIType.TabbedMDI &&
                Frame.Template is MainFormTemplateBase){
                ((MainFormTemplateBase) Frame.Template).DocumentManager.View.DocumentActivated += View_DocumentActivated;
            }
        }

        void View_DocumentActivated(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs e) {
            var templateBase = e.Document.Form as XtraFormTemplateBase;
            if (templateBase != null){
                Frame.OnViewChanged();
            }
        }

        protected override void ShowReminderAlerts(object sender, ReminderEventArgs e) {
            if (_remindersForm == null) {
                _remindersForm = new RemindersForm(Application, (SchedulerStorage)sender);
                _remindersForm.Disposed += RemindersFormDisposed;
            }
            ((System.Windows.Forms.Form)Application.MainWindow.Template).Invoke((Action)(() => _remindersForm.OnReminderAlert(e)));
        }

        void RemindersFormDisposed(object sender, EventArgs e) {
            ((RemindersForm)sender).Disposed -= RemindersFormDisposed;
            _remindersForm = null;
        }
    }
}
