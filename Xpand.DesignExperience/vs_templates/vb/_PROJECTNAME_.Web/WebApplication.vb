
Imports $projectsuffix$.Module.Web
Imports Xpand.ExpressApp.Security.AuthenticationProviders
Imports Microsoft.VisualBasic
Imports System
Imports Xpand.ExpressApp.Web

Partial Public Class $projectsuffix$AspNetApplication
    Inherits XpandWebApplication
    Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
    Private module2 As DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule
    Private module3 As $projectsuffix$.Module.$projectsuffix$Module
    Private module4 As $projectsuffix$AspNetModule
    Private module5 As DevExpress.ExpressApp.Validation.ValidationModule
    Private module6 As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
    Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule

    Friend WithEvents ViewVariantsModule1 As DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule
    Friend WithEvents XpandSystemModule1 As Xpand.ExpressApp.SystemModule.XpandSystemModule
    Friend WithEvents LogicModule1 As Xpand.ExpressApp.Logic.LogicModule
    Friend WithEvents XpandValidationModule1 As Xpand.ExpressApp.Validation.XpandValidationModule
    Friend WithEvents ConditionalControllerStateModule1 As Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule
    Friend WithEvents ConditionalActionStateModule1 As Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule
    Friend WithEvents ModelArtifactStateModule1 As Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule
    Friend WithEvents CloneObjectModule1 As DevExpress.ExpressApp.CloneObject.CloneObjectModule
    Friend WithEvents XpandSecurityModule1 As Xpand.ExpressApp.Security.XpandSecurityModule
    Friend WithEvents ModelDifferenceModule1 As Xpand.ExpressApp.ModelDifference.ModelDifferenceModule
    Friend WithEvents XpandViewVariantsModule1 As Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule
    Friend WithEvents WorldCreatorModule1 As Xpand.ExpressApp.WorldCreator.WorldCreatorModule
    Friend WithEvents TreeListEditorsModuleBase1 As DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase
    Friend WithEvents IoModule1 As Xpand.ExpressApp.IO.IOModule

    Friend WithEvents PivotChartModuleBase1 As DevExpress.ExpressApp.PivotChart.PivotChartModuleBase
    Friend WithEvents XpandPivotChartModule1 As Xpand.ExpressApp.PivotChart.XpandPivotChartModule
    Friend WithEvents FilterDataStoreModule1 As Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule
    Friend WithEvents AdditionalViewControlsModule1 As Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule
    Friend WithEvents WorldCreatorDBMapperModule1 As Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule
    Friend WithEvents ConditionalDetailViewModule1 As Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule

    Friend WithEvents ConditionalAppearanceModule1 As DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule
    Friend WithEvents JobSchedulerModule1 As Xpand.ExpressApp.JobScheduler.JobSchedulerModule
    Friend WithEvents JobSchedulerJobsModule1 As Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule

    Friend WithEvents WorkflowModule1 As DevExpress.ExpressApp.Workflow.WorkflowModule
    Friend WithEvents XpandWorkFlowModule1 As Xpand.ExpressApp.Workflow.XpandWorkFlowModule
    Friend WithEvents StateMachineModule1 As DevExpress.ExpressApp.StateMachine.StateMachineModule
    Friend WithEvents XpandStateMachineModule1 As Xpand.ExpressApp.StateMachine.XpandStateMachineModule
    Friend WithEvents XpandSystemAspNetModule1 As Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule
    Friend WithEvents AdditionalViewControlsProviderAspNetModule1 As Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule
    Friend WithEvents ExceptionHandlingWebModule1 As Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule
    Friend WithEvents FilterDataStoreAspNetModule1 As Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule
    Friend WithEvents TreeListEditorsAspNetModule1 As DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule
    Friend WithEvents FileAttachmentsAspNetModule1 As DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule
    Friend WithEvents IoAspNetModule1 As Xpand.ExpressApp.IO.Web.IOAspNetModule
    Friend WithEvents ModelDifferenceAspNetModule1 As Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule
    Friend WithEvents NCarouselWebModule1 As Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule
    Friend WithEvents PivotChartAspNetModule1 As DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule
    Friend WithEvents XpandPivotChartAspNetModule1 As Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule
    Friend WithEvents ThumbnailWebModule1 As Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule
    Friend WithEvents XpandTreeListEditorsModule1 As Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule
    Friend WithEvents XpandTreeListEditorsAspNetModule1 As Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule
    Friend WithEvents WorldCreatorWebModule1 As Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule
    Friend WithEvents XpandValidationWebModule1 As Xpand.ExpressApp.Validation.Web.XpandValidationWebModule
    Friend WithEvents SecurityStrategyComplex1 As DevExpress.ExpressApp.Security.SecurityStrategyComplex
    Friend WithEvents AuthenticationStandard1 As XpandAuthenticationStandard
    Private sqlConnection1 As System.Data.SqlClient.SqlConnection

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub $projectsuffix$AspNetApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles MyBase.DatabaseVersionMismatch
#If EASYTEST Then
        e.Updater.Update()
        e.Handled = True
#Else
        If System.Diagnostics.Debugger.IsAttached Then
            e.Updater.Update()
            e.Handled = True
        Else
            Dim message As String = "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application." & Constants.vbCrLf & _
                "This error occurred  because the automatic database update was disabled when the application was started without debugging." & Constants.vbCrLf & _
                "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " & _
                "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " & _
                "or manually create a database using the 'DBUpdater' tool." & Constants.vbCrLf & _
                "Anyway, refer to the following help topics for more detailed information:" & Constants.vbCrLf & _
                "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm" & Constants.vbCrLf & _
                "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm" & Constants.vbCrLf & _
                "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/"

            If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
                message &= Constants.vbCrLf & Constants.vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
            End If
            Throw New InvalidOperationException(message)
        End If
#End If
    End Sub

    Private Sub InitializeComponent()
        Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
        Me.module2 = New DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule()
        Me.module3 = New $projectsuffix$.[Module].$projectsuffix$Module()
        Me.module4 = New $projectsuffix$.[Module].Web.$projectsuffix$AspNetModule()
        Me.module5 = New DevExpress.ExpressApp.Validation.ValidationModule()
        Me.module6 = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
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
        Me.XpandSecurityModule1 = New Xpand.ExpressApp.Security.XpandSecurityModule()
        Me.ModelDifferenceModule1 = New Xpand.ExpressApp.ModelDifference.ModelDifferenceModule()
        Me.XpandViewVariantsModule1 = New Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule()
        Me.WorldCreatorModule1 = New Xpand.ExpressApp.WorldCreator.WorldCreatorModule()
        Me.TreeListEditorsModuleBase1 = New DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase()
        Me.IoModule1 = New Xpand.ExpressApp.IO.IOModule()
        Me.PivotChartModuleBase1 = New DevExpress.ExpressApp.PivotChart.PivotChartModuleBase()
        Me.XpandPivotChartModule1 = New Xpand.ExpressApp.PivotChart.XpandPivotChartModule()
        Me.FilterDataStoreModule1 = New Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule()
        Me.AdditionalViewControlsModule1 = New Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule()
        Me.WorldCreatorDBMapperModule1 = New Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule()
        Me.ConditionalDetailViewModule1 = New Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule()
        Me.ConditionalAppearanceModule1 = New DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule()
        Me.JobSchedulerModule1 = New Xpand.ExpressApp.JobScheduler.JobSchedulerModule()
        Me.JobSchedulerJobsModule1 = New Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule()
        Me.WorkflowModule1 = New DevExpress.ExpressApp.Workflow.WorkflowModule()
        Me.XpandWorkFlowModule1 = New Xpand.ExpressApp.Workflow.XpandWorkFlowModule()
        Me.StateMachineModule1 = New DevExpress.ExpressApp.StateMachine.StateMachineModule()
        Me.XpandStateMachineModule1 = New Xpand.ExpressApp.StateMachine.XpandStateMachineModule()
        Me.XpandSystemAspNetModule1 = New Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule()
        Me.AdditionalViewControlsProviderAspNetModule1 = New Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule()
        Me.ExceptionHandlingWebModule1 = New Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule()
        Me.FilterDataStoreAspNetModule1 = New Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule()
        Me.TreeListEditorsAspNetModule1 = New DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule()
        Me.FileAttachmentsAspNetModule1 = New DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule()
        Me.IoAspNetModule1 = New Xpand.ExpressApp.IO.Web.IOAspNetModule()
        Me.ModelDifferenceAspNetModule1 = New Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule()
        Me.NCarouselWebModule1 = New Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule()
        Me.PivotChartAspNetModule1 = New DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule()
        Me.XpandPivotChartAspNetModule1 = New Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule()
        Me.ThumbnailWebModule1 = New Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule()
        Me.XpandTreeListEditorsModule1 = New Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule()
        Me.XpandTreeListEditorsAspNetModule1 = New Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule()
        Me.WorldCreatorWebModule1 = New Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule()
        Me.XpandValidationWebModule1 = New Xpand.ExpressApp.Validation.Web.XpandValidationWebModule()
        Me.SecurityStrategyComplex1 = New DevExpress.ExpressApp.Security.SecurityStrategyComplex()
        Me.AuthenticationStandard1 = New Xpand.ExpressApp.Security.AuthenticationProviders.XpandAuthenticationStandard()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'module5
        '
        Me.module5.AllowValidationDetailsAccess = True
        '
        'sqlConnection1
        '
        Me.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=$projectsuffix$;Integrated Security=SSPI;Pooling=" & _
    "false"
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
        'SecurityStrategyComplex1
        '
        Me.SecurityStrategyComplex1.Authentication = Me.AuthenticationStandard1
        Me.SecurityStrategyComplex1.RoleType = GetType(Xpand.ExpressApp.Security.Core.XpandRole)
        Me.SecurityStrategyComplex1.UserType = GetType(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser)
        '
        'AuthenticationStandard1
        '
        Me.AuthenticationStandard1.LogonParametersType = GetType(Xpand.ExpressApp.Security.AuthenticationProviders.XpandLogonParameters)
        '
        '$projectsuffix$AspNetApplication
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
        Me.Modules.Add(Me.PivotChartModuleBase1)
        Me.Modules.Add(Me.XpandPivotChartModule1)
        Me.Modules.Add(Me.FilterDataStoreModule1)
        Me.Modules.Add(Me.AdditionalViewControlsModule1)
        Me.Modules.Add(Me.WorldCreatorDBMapperModule1)
        Me.Modules.Add(Me.ConditionalDetailViewModule1)
        Me.Modules.Add(Me.ConditionalAppearanceModule1)
        Me.Modules.Add(Me.JobSchedulerModule1)
        Me.Modules.Add(Me.JobSchedulerJobsModule1)
        Me.Modules.Add(Me.WorkflowModule1)
        Me.Modules.Add(Me.XpandWorkFlowModule1)
        Me.Modules.Add(Me.StateMachineModule1)
        Me.Modules.Add(Me.XpandStateMachineModule1)
        Me.Modules.Add(Me.module3)
        Me.Modules.Add(Me.XpandSystemAspNetModule1)
        Me.Modules.Add(Me.AdditionalViewControlsProviderAspNetModule1)
        Me.Modules.Add(Me.ExceptionHandlingWebModule1)
        Me.Modules.Add(Me.FilterDataStoreAspNetModule1)
        Me.Modules.Add(Me.TreeListEditorsAspNetModule1)
        Me.Modules.Add(Me.FileAttachmentsAspNetModule1)
        Me.Modules.Add(Me.IoAspNetModule1)
        Me.Modules.Add(Me.ModelDifferenceAspNetModule1)
        Me.Modules.Add(Me.NCarouselWebModule1)
        Me.Modules.Add(Me.PivotChartAspNetModule1)
        Me.Modules.Add(Me.XpandPivotChartAspNetModule1)
        Me.Modules.Add(Me.ThumbnailWebModule1)
        Me.Modules.Add(Me.XpandTreeListEditorsModule1)
        Me.Modules.Add(Me.XpandTreeListEditorsAspNetModule1)
        Me.Modules.Add(Me.WorldCreatorWebModule1)
        Me.Modules.Add(Me.XpandValidationWebModule1)
        Me.Modules.Add(Me.module4)
        Me.Security = Me.SecurityStrategyComplex1
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
End Class

