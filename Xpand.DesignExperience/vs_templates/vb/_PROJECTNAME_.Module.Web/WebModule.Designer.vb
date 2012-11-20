Imports Microsoft.VisualBasic
Imports System

Partial Public Class $projectsuffix$AspNetModule
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
        '$projectsuffix$AspNetModule
        '
		Me.RequiredModuleTypes.Add(GetType($projectsuffix$.[Module].$projectsuffix$Module))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.IO.Web.IOAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule))
        Me.RequiredModuleTypes.Add(GetType(Xpand.ExpressApp.Validation.Web.XpandValidationWebModule))

    End Sub

#End Region
End Class
