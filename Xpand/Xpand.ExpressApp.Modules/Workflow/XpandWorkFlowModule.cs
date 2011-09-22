using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Workflow {
    [ToolboxItem(true)]
    public sealed partial class XpandWorkFlowModule : XpandModuleBase {
        readonly List<ModuleBase> _serverApplicationModules = new List<ModuleBase>();

        public XpandWorkFlowModule() {
            InitializeComponent();
        }

        public List<ModuleBase> ServerApplicationModules {
            get { return _serverApplicationModules; }
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            WorkflowServerStarter starter = null;
            string setting = ConfigurationManager.AppSettings["WorkflowServerStarter"];
            if (!string.IsNullOrEmpty(setting) && bool.Parse(setting)) {
                Application.LoggedOn += delegate {
                    if (starter == null) {
                        starter = new WorkflowServerStarter();
                        starter.OnCustomHandleException += (sender1, args1) => Tracing.Tracer.LogError(args1.Message);
                        starter.Start(Application.ConnectionString, Application.ApplicationName, _serverApplicationModules);
                    }
                };
            }
        }
    }
}