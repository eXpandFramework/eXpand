#Timeout 160
#DropDB XpandFeatureCenterEasyTest
#DropDB NorthWind
#DropDB XpandFeatureCenterWorldCreatorWeb
#DropDB XpandFeatureCenterWorldCreatorWin
#Application FeatureCenter.Win
#Application FeatureCenter.Web
 
*ScreenCapture
*LogOn

*Action Navigation(WorldCreator.Persistent Assembly Info)

#IfDef FeatureCenter.Win
*XpandProcessRecord 
 Name = MasterDetailDynamicAssembly
!FieldIsVisible
 Fields = Errors 
*SaveAndClose
#EndIf

*XpandProcessRecord 
 Name = [MappedDBName]
!FieldIsVisible
 Fields = Errors 
*SaveAndClose

