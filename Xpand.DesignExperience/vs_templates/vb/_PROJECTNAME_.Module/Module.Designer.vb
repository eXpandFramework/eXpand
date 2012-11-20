Imports Microsoft.VisualBasic
Imports DevExpress.Xpo
Imports Xpand.ExpressApp.ConditionalDetailViews
Imports Xpand.ExpressApp.WorldCreator.DBMapper
Imports Xpand.Persistent.BaseImpl.ExceptionHandling
Imports Xpand.Persistent.BaseImpl.PersistentMetaData
Imports System.Linq
Imports Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos

Partial Public Class $projectsuffix$Module
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
        '
        '
        '
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.SystemModule.SystemModule))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Validation.ValidationModule))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.SystemModule.XpandSystemModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ModelDifference.ModelDifferenceModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Security.XpandSecurityModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Validation.XpandValidationModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.WorldCreator.WorldCreatorModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.IO.IOModule))

        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.PivotChart.XpandPivotChartModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule))

        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.CloneObject.CloneObjectModule))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.JobScheduler.JobSchedulerModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Workflow.XpandWorkFlowModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.StateMachine.XpandStateMachineModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Validation.XpandValidationModule))


    End Sub

#End Region
End Class
