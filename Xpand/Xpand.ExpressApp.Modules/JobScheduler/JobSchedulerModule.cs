using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using System.Linq;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler {
    public interface IModelOptionsJobScheduler:IModelOptions {
        [Category("eXpand.JobScheduler")]
        bool JobScheduler { get; set; }
    }
    [ToolboxBitmap(typeof(JobSchedulerModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsProvider.AdditionalViewControlsModule));
            XafTypesInfo.Instance.LoadTypes(typeof(AnnualCalendar).Assembly);
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsJobScheduler>();
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application == null)
                return;

            if (RuntimeMode) {
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.JobScheduler");
                Application.LoggedOn += ApplicationOnLoggedOn;
            }

        }

        bool Enabled() {
            var modelOptionsJobScheduler = Application.Model.Options as IModelOptionsJobScheduler;
            return modelOptionsJobScheduler != null && modelOptionsJobScheduler.JobScheduler;
        }

        void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            Application.LoggedOn-=ApplicationOnLoggedOn;
            if (!Enabled())
                return;
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(Application);
            try {
                IScheduler scheduler = stdSchedulerFactory.AllSchedulers.SingleOrDefault();
                _scheduler = scheduler ?? stdSchedulerFactory.GetScheduler();
            } catch (Exception e) {
                if (!Debugger.IsAttached)
                    Tracing.Tracer.LogError(e);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && Scheduler is StdScheduler) {
                Scheduler.Shutdown();
            }
        }
        IScheduler _scheduler;
        public IScheduler Scheduler {
            get { return _scheduler; }
        }

    }
}