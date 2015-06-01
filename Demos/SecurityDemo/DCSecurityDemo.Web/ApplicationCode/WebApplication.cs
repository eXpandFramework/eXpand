using DCSecurityDemo.Module;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web;

namespace DCSecurityDemo.Web {
    public partial class DCSecurityDemoAspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;
        private DCSecurityDemo.Module.DCSecurityDemoModule module3;
        private SecurityDemo.Module.Web.SecurityDemoAspNetModule module4;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule module8;
        private DCSecurityDemo.Module.DCSecurityDemoAuthentication authentication1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
        
        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule additionalViewControlsModule1;
        private Xpand.ExpressApp.JobScheduler.JobSchedulerModule jobSchedulerModule1;
        
        private Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule modelArtifactStateModule1;
        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.StateMachine.StateMachineModule stateMachineModule1;
        private Xpand.ExpressApp.StateMachine.XpandStateMachineModule xpandStateMachineModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule xpandViewVariantsModule1;
        private DevExpress.ExpressApp.Workflow.WorkflowModule workflowModule1;
        private Xpand.ExpressApp.Workflow.XpandWorkFlowModule xpandWorkFlowModule1;
        private Xpand.ExpressApp.WorldCreator.WorldCreatorModule worldCreatorModule1;
        private Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule worldCreatorDBMapperModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        private Xpand.ExpressApp.ModelDifference.ModelDifferenceModule modelDifferenceModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule additionalViewControlsProviderAspNetModule1;
        private Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule filterDataStoreModule1;
        private Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule filterDataStoreAspNetModule1;
        private Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule modelDifferenceAspNetModule1;
        private Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule nCarouselWebModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private Xpand.ExpressApp.PivotChart.XpandPivotChartModule xpandPivotChartModule1;
        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule xpandPivotChartAspNetModule1;
        private Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule xpandTreeListEditorsModule1;
        private Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule xpandTreeListEditorsAspNetModule1;
        private Xpand.ExpressApp.Validation.Web.XpandValidationWebModule xpandValidationWebModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule1;
        private Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule worldCreatorWebModule1;
        private Xpand.ExpressApp.IO.IOModule ioModule1;
        private Xpand.ExpressApp.IO.Web.IOAspNetModule ioAspNetModule1;
        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
        private Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule xpandSystemAspNetModule1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityComplex1;

        public DCSecurityDemoAspNetApplication() {
            InitializeComponent();
        }

        private void DCSecurityDemoAspNetApplication_DatabaseVersionMismatch(object sender2, DatabaseVersionMismatchEventArgs e) {
            try {
                e.Updater.Update();
                e.Handled = true;
            } catch (CompatibilityException exception) {
                if (exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
        private void DCSecurityDemoAspNetApplication_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(CreateController<ShowHintController>());
        }
        private void DCSecurityDemoAspNetApplication_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = CreateObjectSpace();
            ((DCSecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module3 = new DCSecurityDemo.Module.DCSecurityDemoModule();
            this.module4 = new SecurityDemo.Module.Web.SecurityDemoAspNetModule();
            this.module8 = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.validationModule = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.authentication1 = new DCSecurityDemo.Module.DCSecurityDemoAuthentication();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.logicModule1 = new Xpand.ExpressApp.Logic.LogicModule();
            
            this.xpandValidationModule1 = new Xpand.ExpressApp.Validation.XpandValidationModule();
            this.additionalViewControlsModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule();
            this.jobSchedulerModule1 = new Xpand.ExpressApp.JobScheduler.JobSchedulerModule();
            
            this.modelArtifactStateModule1 = new Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule();
            this.xpandSecurityModule1 = new Xpand.ExpressApp.Security.XpandSecurityModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.stateMachineModule1 = new DevExpress.ExpressApp.StateMachine.StateMachineModule();
            this.xpandStateMachineModule1 = new Xpand.ExpressApp.StateMachine.XpandStateMachineModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.xpandViewVariantsModule1 = new Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule();
            this.workflowModule1 = new DevExpress.ExpressApp.Workflow.WorkflowModule();
            this.xpandWorkFlowModule1 = new Xpand.ExpressApp.Workflow.XpandWorkFlowModule();
            this.worldCreatorModule1 = new Xpand.ExpressApp.WorldCreator.WorldCreatorModule();
            this.worldCreatorDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.modelDifferenceModule1 = new Xpand.ExpressApp.ModelDifference.ModelDifferenceModule();
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.additionalViewControlsProviderAspNetModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule();
            this.filterDataStoreModule1 = new Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule();
            this.filterDataStoreAspNetModule1 = new Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule();
            this.modelDifferenceAspNetModule1 = new Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule();
            this.nCarouselWebModule1 = new Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.xpandPivotChartModule1 = new Xpand.ExpressApp.PivotChart.XpandPivotChartModule();
            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.xpandPivotChartAspNetModule1 = new Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule();
            this.xpandTreeListEditorsModule1 = new Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule();
            this.xpandTreeListEditorsAspNetModule1 = new Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule();
            this.xpandValidationWebModule1 = new Xpand.ExpressApp.Validation.Web.XpandValidationWebModule();
            this.fileAttachmentsAspNetModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.worldCreatorWebModule1 = new Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule();
            this.ioModule1 = new Xpand.ExpressApp.IO.IOModule();
            this.ioAspNetModule1 = new Xpand.ExpressApp.IO.Web.IOAspNetModule();
            this.xpandSystemModule1 = new Xpand.ExpressApp.SystemModule.XpandSystemModule();
            this.xpandSystemAspNetModule1 = new Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module3
            // 
            this.module3.AdditionalControllerTypes.Add(typeof(DevExpress.ExpressApp.Demos.ShowHintController));
            // 
            // validationModule
            // 
            this.validationModule.AllowValidationDetailsAccess = true;
            // 
            // securityComplex1
            // 
            this.securityComplex1.Authentication = this.authentication1;
            this.securityComplex1.RoleType = typeof(DCSecurityDemo.Module.Security.IDCRole);
            this.securityComplex1.UserType = typeof(DCSecurityDemo.Module.Security.IDCUser);
            // 
            // stateMachineModule1
            // 
            this.stateMachineModule1.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
            // 
            // viewVariantsModule1
            // 
            this.viewVariantsModule1.GenerateVariantsNode = true;
            this.viewVariantsModule1.ShowAdditionalNavigation = false;
            // 
            // workflowModule1
            // 
            this.workflowModule1.RunningWorkflowInstanceInfoType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoRunningWorkflowInstanceInfo);
            this.workflowModule1.StartWorkflowRequestType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoStartWorkflowRequest);
            this.workflowModule1.UserActivityVersionType = typeof(DevExpress.ExpressApp.Workflow.Versioning.XpoUserActivityVersion);
            this.workflowModule1.WorkflowControlCommandRequestType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowInstanceControlCommandRequest);
            this.workflowModule1.WorkflowDefinitionType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowDefinition);
            this.workflowModule1.WorkflowInstanceKeyType = typeof(DevExpress.Workflow.Xpo.XpoInstanceKey);
            this.workflowModule1.WorkflowInstanceType = typeof(DevExpress.Workflow.Xpo.XpoWorkflowInstance);
            // 
            // pivotChartModuleBase1
            // 
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // DCSecurityDemoAspNetApplication
            // 
            this.ApplicationName = "DCSecurityDemo";
            this.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=D" +
    "CSecurityDemo_v12.2";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.logicModule1);
            
            this.Modules.Add(this.validationModule);
            this.Modules.Add(this.xpandValidationModule1);
            this.Modules.Add(this.additionalViewControlsModule1);
            this.Modules.Add(this.jobSchedulerModule1);
            
            this.Modules.Add(this.modelArtifactStateModule1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.stateMachineModule1);
            this.Modules.Add(this.xpandStateMachineModule1);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.xpandViewVariantsModule1);
            this.Modules.Add(this.workflowModule1);
            this.Modules.Add(this.xpandWorkFlowModule1);
            this.Modules.Add(this.worldCreatorModule1);
            this.Modules.Add(this.worldCreatorDBMapperModule1);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.treeListEditorsModuleBase1);
            this.Modules.Add(this.module8);
            this.Modules.Add(this.additionalViewControlsProviderAspNetModule1);
            this.Modules.Add(this.filterDataStoreModule1);
            this.Modules.Add(this.filterDataStoreAspNetModule1);
            this.Modules.Add(this.modelDifferenceAspNetModule1);
            this.Modules.Add(this.nCarouselWebModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.xpandPivotChartModule1);
            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.xpandPivotChartAspNetModule1);
            this.Modules.Add(this.xpandTreeListEditorsModule1);
            this.Modules.Add(this.xpandTreeListEditorsAspNetModule1);
            this.Modules.Add(this.xpandValidationWebModule1);
            this.Modules.Add(this.fileAttachmentsAspNetModule1);
            this.Modules.Add(this.worldCreatorWebModule1);
            this.Modules.Add(this.ioModule1);
            this.Modules.Add(this.ioAspNetModule1);
            this.Modules.Add(this.xpandSystemModule1);
            this.Modules.Add(this.xpandSystemAspNetModule1);
            this.Modules.Add(this.module4);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.DCSecurityDemoAspNetApplication_DatabaseVersionMismatch);
            this.CreateCustomLogonWindowControllers += new System.EventHandler<DevExpress.ExpressApp.CreateCustomLogonWindowControllersEventArgs>(this.DCSecurityDemoAspNetApplication_CreateCustomLogonWindowControllers);
            this.CreateCustomLogonWindowObjectSpace += new System.EventHandler<DevExpress.ExpressApp.CreateCustomLogonWindowObjectSpaceEventArgs>(this.DCSecurityDemoAspNetApplication_CreateCustomLogonWindowObjectSpace);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}
