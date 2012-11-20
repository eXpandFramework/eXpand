Imports $projectsuffix$.Module.Win
Imports Xpand.ExpressApp.Security.AuthenticationProviders
Imports Xpand.ExpressApp.FilterDataStore.Win

Partial Public Class $projectsuffix$WindowsFormsApplication
    ''' <summary> 
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary> 
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (Not components Is Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Component Designer generated code"

    ''' <summary> 
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
        Me.module2 = New DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule()
        Me.module3 = New $projectsuffix$.[Module].$projectsuffix$Module()
        Me.module4 = New $projectsuffix$WindowsFormsModule()
        Me.module5 = New DevExpress.ExpressApp.Validation.ValidationModule()
        Me.module6 = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
        Me.module7 = New DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule()
        Me.securityModule1 = New DevExpress.ExpressApp.Security.SecurityModule()
        Me.sqlConnection1 = New System.Data.SqlClient.SqlConnection()
        Me.ViewVariantsModule1 = New DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule()
        Me.XpandSystemModule1 = New Xpand.ExpressApp.SystemModule.XpandSystemModule()
        Me.LogicModule1 = New Xpand.ExpressApp.Logic.LogicModule()
        Me.XpandValidationModule1 = New Xpand.ExpressApp.Validation.XpandValidationModule()
        Me.ConditionalControllerStateModule1 = New Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule()
        Me.ConditionalActionStateModule1 = New Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule()
        Me.ModelArtifactStateModule1 = New Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule()
        Me.CloneObjectModule1 = New DevExpress.ExpressApp.CloneObject.CloneObjectModule()
        Me.ModelDifferenceModule1 = New Xpand.ExpressApp.ModelDifference.ModelDifferenceModule()
        Me.XpandSecurityModule1 = New Xpand.ExpressApp.Security.XpandSecurityModule()
        Me.XpandViewVariantsModule1 = New Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule()
        Me.WorldCreatorModule1 = New Xpand.ExpressApp.WorldCreator.WorldCreatorModule()
        Me.TreeListEditorsModuleBase1 = New DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase()
        Me.IoModule1 = New Xpand.ExpressApp.IO.IOModule()
        Me.MasterDetailModule1 = New Xpand.ExpressApp.MasterDetail.MasterDetailModule()
        Me.PivotChartModuleBase1 = New DevExpress.ExpressApp.PivotChart.PivotChartModuleBase()
        Me.XpandPivotChartModule1 = New Xpand.ExpressApp.PivotChart.XpandPivotChartModule()
        Me.FilterDataStoreModule1 = New Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule()
        Me.AdditionalViewControlsModule1 = New Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule()
        Me.ConditionalDetailViewModule1 = New Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule()

        Me.FileAttachmentsWindowsFormsModule1 = New DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule()
        Me.HtmlPropertyEditorWindowsFormsModule1 = New DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule()
        Me.PivotChartWindowsFormsModule1 = New DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule()
        Me.ReportsModule1 = New DevExpress.ExpressApp.Reports.ReportsModule()
        Me.ReportsWindowsFormsModule1 = New DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule()
        Me.SchedulerModuleBase1 = New DevExpress.ExpressApp.Scheduler.SchedulerModuleBase()
        Me.SchedulerWindowsFormsModule1 = New DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule()
        Me.TreeListEditorsWindowsFormsModule1 = New DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule()
        Me.ModelDifferenceWindowsFormsModule1 = New Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule()
        Me.XpandTreeListEditorsWinModule1 = New Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule()
        Me.XpandSystemWindowsFormsModule1 = New Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule()
        Me.WizardUIWindowsFormsModule1 = New Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule()
        Me.WorldCreatorWinModule1 = New Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule()
        Me.IoWinModule1 = New Xpand.ExpressApp.IO.Win.IOWinModule()
        Me.ExceptionHandlingWinModule1 = New Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule()
        Me.FilterDataStoreWindowsFormsModule1 = New Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule()
        Me.AdditionalViewControlsProviderWindowsFormsModule1 = New Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule()
        Me.XpandPivotChartWinModule1 = New Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule()
        Me.LogicWindowsModule1 = New Xpand.ExpressApp.Logic.Win.LogicWindowsModule()
        Me.MasterDetailWindowsModule1 = New Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule()

        Me.WorldCreatorDBMapperModule1 = New Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule()
        Me.ConditionalAppearanceModule1 = New DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule()
        Me.JobSchedulerModule1 = New Xpand.ExpressApp.JobScheduler.JobSchedulerModule()
        Me.JobSchedulerJobsModule1 = New Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule()
        Me.ImportWizardModule1 = New Xpand.ExpressApp.ImportWizard.ImportWizardModule()
        Me.WorkflowModule1 = New DevExpress.ExpressApp.Workflow.WorkflowModule()
        Me.XpandWorkFlowModule1 = New Xpand.ExpressApp.Workflow.XpandWorkFlowModule()
        Me.StateMachineModule1 = New DevExpress.ExpressApp.StateMachine.StateMachineModule()
        Me.XpandStateMachineModule1 = New Xpand.ExpressApp.StateMachine.XpandStateMachineModule()
        Me.XpandTreeListEditorsModule1 = New Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule()
        Me.ImportWizardWindowsFormsModule1 = New Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule()
        Me.XpandValidationWinModule1 = New Xpand.ExpressApp.Validation.Win.XpandValidationWinModule()
        Me.SecurityStrategyComplex1 = New DevExpress.ExpressApp.Security.SecurityStrategyComplex()
        Me.AuthenticationStandard1 = New XpandAuthenticationStandard()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'module5
        '
        Me.module5.AllowValidationDetailsAccess = True
        '
        'sqlConnection1
        '
        Me.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=$projectsuffix$;Integrated Security=SSPI;Pooling=f" & _
    "alse"
        Me.sqlConnection1.FireInfoMessageEventOnUserErrors = False
        '
        'ViewVariantsModule1
        '
        Me.ViewVariantsModule1.GenerateVariantsNode = True
        Me.ViewVariantsModule1.ShowAdditionalNavigation = False
        '
        'PivotChartModuleBase1
        '
        Me.PivotChartModuleBase1.ShowAdditionalNavigation = False
        '
        'ReportsModule1
        '
        Me.ReportsModule1.EnableInplaceReports = True
        Me.ReportsModule1.ReportDataType = GetType(DevExpress.ExpressApp.Reports.ReportData)
        '
        'WorkflowModule1
        '
        Me.WorkflowModule1.RunningWorkflowInstanceInfoType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoRunningWorkflowInstanceInfo)
        Me.WorkflowModule1.StartWorkflowRequestType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoStartWorkflowRequest)
        Me.WorkflowModule1.UserActivityVersionType = GetType(DevExpress.ExpressApp.Workflow.Versioning.XpoUserActivityVersion)
        Me.WorkflowModule1.WorkflowControlCommandRequestType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowInstanceControlCommandRequest)
        Me.WorkflowModule1.WorkflowDefinitionType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowDefinition)
        Me.WorkflowModule1.WorkflowInstanceKeyType = GetType(DevExpress.Workflow.Xpo.XpoInstanceKey)
        Me.WorkflowModule1.WorkflowInstanceType = GetType(DevExpress.Workflow.Xpo.XpoWorkflowInstance)
        '
        'StateMachineModule1
        '
        Me.StateMachineModule1.StateMachineStorageType = GetType(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine)
        '
        'ImportWizardWindowsFormsModule1
        '
        Me.ImportWizardWindowsFormsModule1.ResourcesExportedToModel.Add(GetType(Xpand.ExpressApp.ImportWizard.Win.ImportWizResourceLocalizer))
        Me.ImportWizardWindowsFormsModule1.ResourcesExportedToModel.Add(GetType(Xpand.ExpressApp.ImportWizard.Win.ImportWizFrameTemplateLocalizer))
        '
        'SecurityStrategyComplex1
        '
        Me.SecurityStrategyComplex1.Authentication = Me.AuthenticationStandard1
        Me.SecurityStrategyComplex1.RoleType = GetType(Xpand.ExpressApp.Security.Core.XpandRole)
        Me.SecurityStrategyComplex1.UserType = GetType(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser)
        '
        'AuthenticationStandard1
        '
        Me.AuthenticationStandard1.LogonParametersType = GetType(XpandLogonParameters)
        '
        '$projectsuffix$WindowsFormsApplication
        '
        Me.ApplicationName = "$projectsuffix$"
        Me.Connection = Me.sqlConnection1
        Me.Modules.Add(Me.module1)
        Me.Modules.Add(Me.module2)
        Me.Modules.Add(Me.module6)
        Me.Modules.Add(Me.module5)
        Me.Modules.Add(Me.ViewVariantsModule1)
        Me.Modules.Add(Me.securityModule1)
        Me.Modules.Add(Me.XpandSystemModule1)
        Me.Modules.Add(Me.LogicModule1)
        Me.Modules.Add(Me.XpandValidationModule1)
        Me.Modules.Add(Me.ConditionalControllerStateModule1)
        Me.Modules.Add(Me.ConditionalActionStateModule1)
        Me.Modules.Add(Me.ModelArtifactStateModule1)
        Me.Modules.Add(Me.CloneObjectModule1)
        Me.Modules.Add(Me.XpandSecurityModule1)
        Me.Modules.Add(Me.ModelDifferenceModule1)
        Me.Modules.Add(Me.XpandViewVariantsModule1)
        Me.Modules.Add(Me.WorldCreatorModule1)
        Me.Modules.Add(Me.TreeListEditorsModuleBase1)
        Me.Modules.Add(Me.IoModule1)
        Me.Modules.Add(Me.MasterDetailModule1)
        Me.Modules.Add(Me.PivotChartModuleBase1)
        Me.Modules.Add(Me.XpandPivotChartModule1)
        Me.Modules.Add(Me.FilterDataStoreModule1)
        Me.Modules.Add(Me.AdditionalViewControlsModule1)
        Me.Modules.Add(Me.ConditionalDetailViewModule1)

        Me.Modules.Add(Me.WorldCreatorDBMapperModule1)
        Me.Modules.Add(Me.ConditionalAppearanceModule1)
        Me.Modules.Add(Me.JobSchedulerModule1)
        Me.Modules.Add(Me.JobSchedulerJobsModule1)
        Me.Modules.Add(Me.ImportWizardModule1)
        Me.Modules.Add(Me.WorkflowModule1)
        Me.Modules.Add(Me.XpandWorkFlowModule1)
        Me.Modules.Add(Me.StateMachineModule1)
        Me.Modules.Add(Me.XpandStateMachineModule1)
        Me.Modules.Add(Me.module3)
        Me.Modules.Add(Me.FileAttachmentsWindowsFormsModule1)
        Me.Modules.Add(Me.HtmlPropertyEditorWindowsFormsModule1)
        Me.Modules.Add(Me.PivotChartWindowsFormsModule1)
        Me.Modules.Add(Me.ReportsModule1)
        Me.Modules.Add(Me.ReportsWindowsFormsModule1)
        Me.Modules.Add(Me.SchedulerModuleBase1)
        Me.Modules.Add(Me.SchedulerWindowsFormsModule1)
        Me.Modules.Add(Me.TreeListEditorsWindowsFormsModule1)
        Me.Modules.Add(Me.module7)
        Me.Modules.Add(Me.XpandSystemWindowsFormsModule1)
        Me.Modules.Add(Me.ModelDifferenceWindowsFormsModule1)
        Me.Modules.Add(Me.XpandTreeListEditorsModule1)
        Me.Modules.Add(Me.XpandTreeListEditorsWinModule1)
        Me.Modules.Add(Me.WizardUIWindowsFormsModule1)
        Me.Modules.Add(Me.WorldCreatorWinModule1)
        Me.Modules.Add(Me.IoWinModule1)
        Me.Modules.Add(Me.ExceptionHandlingWinModule1)
        Me.Modules.Add(Me.FilterDataStoreWindowsFormsModule1)
        Me.Modules.Add(Me.AdditionalViewControlsProviderWindowsFormsModule1)
        Me.Modules.Add(Me.XpandPivotChartWinModule1)
        Me.Modules.Add(Me.LogicWindowsModule1)
        Me.Modules.Add(Me.MasterDetailWindowsModule1)

        Me.Modules.Add(Me.ImportWizardWindowsFormsModule1)
        Me.Modules.Add(Me.XpandValidationWinModule1)
        Me.Modules.Add(Me.module4)
        Me.Security = Me.SecurityStrategyComplex1
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

#End Region

    Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
    Private module2 As DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule
    Private module3 As Global.$projectsuffix$.Module.$projectsuffix$Module
    Private module4 As Global.$projectsuffix$.Module.Win.$projectsuffix$WindowsFormsModule
    Private module5 As DevExpress.ExpressApp.Validation.ValidationModule
    Private module6 As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
    Private module7 As DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule
    Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule

    Private _filterDataStoreWindowsFormsModule As FilterDataStoreWindowsFormsModule

    Private sqlConnection1 As System.Data.SqlClient.SqlConnection
    Friend WithEvents ViewVariantsModule1 As DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule
    Friend WithEvents XpandSystemModule1 As Xpand.ExpressApp.SystemModule.XpandSystemModule
    Friend WithEvents LogicModule1 As Xpand.ExpressApp.Logic.LogicModule
    Friend WithEvents XpandValidationModule1 As Xpand.ExpressApp.Validation.XpandValidationModule
    Friend WithEvents ConditionalControllerStateModule1 As Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule
    Friend WithEvents ConditionalActionStateModule1 As Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule
    Friend WithEvents ModelArtifactStateModule1 As Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule
    Friend WithEvents CloneObjectModule1 As DevExpress.ExpressApp.CloneObject.CloneObjectModule
    Friend WithEvents ModelDifferenceModule1 As Xpand.ExpressApp.ModelDifference.ModelDifferenceModule
    Friend WithEvents XpandSecurityModule1 As Xpand.ExpressApp.Security.XpandSecurityModule
    Friend WithEvents XpandViewVariantsModule1 As Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule
    Friend WithEvents WorldCreatorModule1 As Xpand.ExpressApp.WorldCreator.WorldCreatorModule
    Friend WithEvents TreeListEditorsModuleBase1 As DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase
    Friend WithEvents IoModule1 As Xpand.ExpressApp.IO.IOModule
    Friend WithEvents MasterDetailModule1 As Xpand.ExpressApp.MasterDetail.MasterDetailModule
    Friend WithEvents PivotChartModuleBase1 As DevExpress.ExpressApp.PivotChart.PivotChartModuleBase
    Friend WithEvents XpandPivotChartModule1 As Xpand.ExpressApp.PivotChart.XpandPivotChartModule
    Friend WithEvents FilterDataStoreModule1 As Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule
    Friend WithEvents AdditionalViewControlsModule1 As Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule
    Friend WithEvents ConditionalDetailViewModule1 As Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule


    Friend WithEvents FileAttachmentsWindowsFormsModule1 As DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule


    Friend WithEvents HtmlPropertyEditorWindowsFormsModule1 As DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule
    Friend WithEvents PivotChartWindowsFormsModule1 As DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule
    Friend WithEvents ReportsModule1 As DevExpress.ExpressApp.Reports.ReportsModule
    Friend WithEvents ReportsWindowsFormsModule1 As DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule
    Friend WithEvents SchedulerModuleBase1 As DevExpress.ExpressApp.Scheduler.SchedulerModuleBase
    Friend WithEvents SchedulerWindowsFormsModule1 As DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule
    Friend WithEvents TreeListEditorsWindowsFormsModule1 As DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule

    Friend WithEvents ModelDifferenceWindowsFormsModule1 As Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule
    Friend WithEvents XpandTreeListEditorsWinModule1 As Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule
    Friend WithEvents XpandSystemWindowsFormsModule1 As Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule
    Friend WithEvents WizardUIWindowsFormsModule1 As Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule
    Friend WithEvents WorldCreatorWinModule1 As Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule
    Friend WithEvents IoWinModule1 As Xpand.ExpressApp.IO.Win.IOWinModule
    Friend WithEvents ExceptionHandlingWinModule1 As Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule
    Friend WithEvents FilterDataStoreWindowsFormsModule1 As Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule
    Friend WithEvents AdditionalViewControlsProviderWindowsFormsModule1 As Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule
    Friend WithEvents XpandPivotChartWinModule1 As Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule
    Friend WithEvents LogicWindowsModule1 As Xpand.ExpressApp.Logic.Win.LogicWindowsModule
    Friend WithEvents MasterDetailWindowsModule1 As Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule

    Friend WithEvents WorldCreatorDBMapperModule1 As Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule
    Friend WithEvents ConditionalAppearanceModule1 As DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule
    Friend WithEvents JobSchedulerModule1 As Xpand.ExpressApp.JobScheduler.JobSchedulerModule
    Friend WithEvents JobSchedulerJobsModule1 As Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule
    Friend WithEvents ImportWizardModule1 As Xpand.ExpressApp.ImportWizard.ImportWizardModule
    Friend WithEvents WorkflowModule1 As DevExpress.ExpressApp.Workflow.WorkflowModule
    Friend WithEvents XpandWorkFlowModule1 As Xpand.ExpressApp.Workflow.XpandWorkFlowModule
    Friend WithEvents StateMachineModule1 As DevExpress.ExpressApp.StateMachine.StateMachineModule
    Friend WithEvents XpandStateMachineModule1 As Xpand.ExpressApp.StateMachine.XpandStateMachineModule
    Friend WithEvents XpandTreeListEditorsModule1 As Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule
    Friend WithEvents ImportWizardWindowsFormsModule1 As Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule
    Friend WithEvents XpandValidationWinModule1 As Xpand.ExpressApp.Validation.Win.XpandValidationWinModule
    Friend WithEvents SecurityStrategyComplex1 As DevExpress.ExpressApp.Security.SecurityStrategyComplex
    Friend WithEvents AuthenticationStandard1 As XpandAuthenticationStandard
End Class
