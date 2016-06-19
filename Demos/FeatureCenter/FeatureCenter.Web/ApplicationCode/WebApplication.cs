using System.ComponentModel;
using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.ExpressApp.Kpi;
using DevExpress.ExpressApp.Objects;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Web;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Reports.Web;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.ExpressApp.ScriptRecorder;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.TreeListEditors;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Workflow.Xpo;
using FeatureCenter.Module;
using FeatureCenter.Module.Web;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Web;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Web.ApplicationCode{
    public class FeatureCenterAspNetApplication : XpandWebApplication{
        private SystemModule _module1;
        private SystemAspNetModule _module2;

        private SecurityModule _securityModule1;
        private BusinessClassLibraryCustomizationModule _module6;
        private SqlConnection _sqlConnection1;
        private SecurityStrategyComplex _securityComplex1;
        private XpandAuthenticationStandard _authenticationStandard1;
        private FeatureCenterModule _featureCenterModule1;
        private CloneObjectModule _cloneObjectModule1;
        private ViewVariantsModule _viewVariantsModule1;


        private PivotChartModuleBase _pivotChartModuleBase1;

        private ScriptRecorderModuleBase _scriptRecorderModuleBase1;


        private FeatureCenterAspNetModule _featureCenterAspNetModule1;


        private PivotChartAspNetModule _pivotChartAspNetModule1;
        private FileAttachmentsAspNetModule _fileAttachmentsAspNetModule1;

        private ConditionalAppearanceModule _conditionalAppearanceModule1;
        private KpiModule _kpiModule1;

        private WorkflowModule _workflowModule1;
        private HtmlPropertyEditorAspNetModule _htmlPropertyEditorAspNetModule1;
        private ReportsModule _reportsModule1;
        private ReportsAspNetModule _reportsAspNetModule1;
        private TreeListEditorsAspNetModule _treeListEditorsAspNetModule1;
        private SchedulerModuleBase _schedulerModuleBase1;
        private SchedulerAspNetModule _schedulerAspNetModule1;
        private StateMachineModule _stateMachineModule1;
        private TreeListEditorsModuleBase _treeListEditorsModuleBase2;
        private ValidationModule _module5;

        public FeatureCenterAspNetApplication(){
            InitializeComponent();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(args.ConnectionString));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider());
        }


#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif

        private void AspNetApplicationDatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e){
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (true){
                if (this.DropDatabaseOnVersionMissmatch() > 0)
                    Exit();
                e.Updater.ForceUpdateDatabase = true;
                e.Updater.Update();
                e.Handled = true;
            }
#endif
        }


        private void InitializeComponent(){
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module5 = new ValidationModule();
            _module6 = new BusinessClassLibraryCustomizationModule();
            _securityModule1 = new SecurityModule();
            _sqlConnection1 = new SqlConnection();
            _securityComplex1 = new SecurityStrategyComplex();
            _authenticationStandard1 = new XpandAuthenticationStandard();
            _featureCenterModule1 = new FeatureCenterModule();
            _cloneObjectModule1 = new CloneObjectModule();
            _viewVariantsModule1 = new ViewVariantsModule();
            _pivotChartModuleBase1 = new PivotChartModuleBase();
            _scriptRecorderModuleBase1 = new ScriptRecorderModuleBase();
            _featureCenterAspNetModule1 = new FeatureCenterAspNetModule();
            _pivotChartAspNetModule1 = new PivotChartAspNetModule();
            _fileAttachmentsAspNetModule1 = new FileAttachmentsAspNetModule();
            _conditionalAppearanceModule1 = new ConditionalAppearanceModule();
            _kpiModule1 = new KpiModule();
            _workflowModule1 = new WorkflowModule();
            _htmlPropertyEditorAspNetModule1 = new HtmlPropertyEditorAspNetModule();
            _reportsModule1 = new ReportsModule();
            _reportsAspNetModule1 = new ReportsAspNetModule();
            _treeListEditorsAspNetModule1 = new TreeListEditorsAspNetModule();
            _schedulerModuleBase1 = new SchedulerModuleBase();
            _schedulerAspNetModule1 = new SchedulerAspNetModule();
            _stateMachineModule1 = new StateMachineModule();
            _treeListEditorsModuleBase2 = new TreeListEditorsModuleBase();
            ((ISupportInitialize) this).BeginInit();
            // 
            // module5
            // 
            _module5.AllowValidationDetailsAccess = true;
            _module5.IgnoreWarningAndInformationRules = false;
            // 
            // securityModule1
            // 
            _securityModule1.UserType = typeof(SecuritySystemUser);
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                "Data Source=(local);Initial Catalog=XpandFeatureCenter;Integrated Security=SSPI;P" +
                "ooling=false";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityComplex1
            // 
            _securityComplex1.Authentication = _authenticationStandard1;
            _securityComplex1.RoleType = typeof(XpandRole);
            _securityComplex1.UserType = typeof(SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            _authenticationStandard1.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // viewVariantsModule1
            // 
            _viewVariantsModule1.FrameVariantsEngine = null;
            _viewVariantsModule1.VariantsProvider = null;
            // 
            // pivotChartModuleBase1
            // 
            _pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // workflowModule1
            // 
            _workflowModule1.RunningWorkflowInstanceInfoType = typeof(XpoRunningWorkflowInstanceInfo);
            _workflowModule1.StartWorkflowRequestType = typeof(XpoStartWorkflowRequest);
            _workflowModule1.UserActivityVersionType = typeof(XpoUserActivityVersion);
            _workflowModule1.WorkflowControlCommandRequestType = typeof(XpoWorkflowInstanceControlCommandRequest);
            _workflowModule1.WorkflowDefinitionType = typeof(XpoWorkflowDefinition);
            _workflowModule1.WorkflowInstanceKeyType = typeof(XpoInstanceKey);
            _workflowModule1.WorkflowInstanceType = typeof(XpoWorkflowInstance);
            // 
            // reportsModule1
            // 
            _reportsModule1.EnableInplaceReports = true;
            _reportsModule1.ReportDataType = typeof(ReportData);
            // 
            // stateMachineModule1
            // 
            _stateMachineModule1.StateMachineStorageType = typeof(XpoStateMachine);
            // 
            // FeatureCenterAspNetApplication
            // 
            ApplicationName = "FeatureCenter";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module5);
            Modules.Add(_module6);
            Modules.Add(_securityModule1);
            Modules.Add(_cloneObjectModule1);
            Modules.Add(_viewVariantsModule1);
            Modules.Add(_conditionalAppearanceModule1);
            Modules.Add(_pivotChartModuleBase1);
            Modules.Add(_scriptRecorderModuleBase1);
            Modules.Add(_kpiModule1);
            Modules.Add(_workflowModule1);
            Modules.Add(_stateMachineModule1);
            Modules.Add(_featureCenterModule1);
            Modules.Add(_pivotChartAspNetModule1);
            Modules.Add(_fileAttachmentsAspNetModule1);
            Modules.Add(_htmlPropertyEditorAspNetModule1);
            Modules.Add(_reportsModule1);
            Modules.Add(_reportsAspNetModule1);
            Modules.Add(_treeListEditorsModuleBase2);
            Modules.Add(_treeListEditorsAspNetModule1);
            Modules.Add(_schedulerModuleBase1);
            Modules.Add(_schedulerAspNetModule1);
            Modules.Add(_featureCenterAspNetModule1);
            Security = _securityComplex1;
            DatabaseVersionMismatch += AspNetApplicationDatabaseVersionMismatch;
            ((ISupportInitialize) this).EndInit();
        }
    }
}