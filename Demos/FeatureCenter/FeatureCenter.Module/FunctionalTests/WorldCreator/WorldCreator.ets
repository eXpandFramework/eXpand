#Timeout 160
#DropDB XpandFeatureCenterEasyTest
#DropDB NorthWind
#DropDB XpandFeatureCenterWorldCreator
#Application FeatureCenter.Win
#Application FeatureCenter.Web
 
*ScreenCapture
*LogOn

#IfDef FeatureCenter.Win
*Action Navigation(WorldCreator.Persistent Assembly Info)
*XpandProcessRecord 
 Name = MasterDetailDynamicAssembly
!FieldIsVisible
 Fields = Errors 
*SaveAndClose
#EndIf

*ProcessRecord 
 Name = [MappedDBName]
!FieldIsVisible
 Fields = Errors 
*SaveAndClose

