
Imports DevExpress.ExpressApp
Imports System.Reflection
Imports DevExpress.Persistent.BaseImpl
Imports Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation

Partial Public NotInheritable Class [$projectsuffix$Module]
    Inherits ModuleBase
    Public Sub New()
        AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(GetType(Analysis)), AddressOf IsExportedType))
        AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(GetType(Xpand.Persistent.BaseImpl.Updater)), AddressOf IsExportedType))
        AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(GetType(ThresholdSeverity)), AddressOf IsExportedType))
        InitializeComponent()
    End Sub

End Class
