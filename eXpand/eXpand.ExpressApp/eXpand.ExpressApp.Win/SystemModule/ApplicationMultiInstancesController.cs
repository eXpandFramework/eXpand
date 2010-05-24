using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsApplicationMultiInstances:IModelNode {
        [Category("eXpand")]
        [Description("If false only one application instance is allowed")]
        bool ApplicationMultiInstances { get; set; }
    }
    public class ApplicationMultiInstancesController:WindowController,IModelExtender {
        public ApplicationMultiInstancesController() {
            TargetWindowType=WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (((IModelOptionsApplicationMultiInstances) Application.Model.Options).ApplicationMultiInstances) {
                string processName = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 1) {
                    if (processes.Where(process => !process.Equals(Process.GetCurrentProcess())).FirstOrDefault()!=null){
                        MessageBox.Show("Application is already running");
                        Environment.Exit(0);
                    }
                }
            }
        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsApplicationMultiInstances>();
        }

        #endregion
    }
}