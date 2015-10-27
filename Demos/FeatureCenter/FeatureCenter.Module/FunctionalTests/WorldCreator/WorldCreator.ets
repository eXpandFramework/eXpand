#Timeout 160
#DropDB XpandFeatureCenterEasyTest
#DropDB NorthWind
#DropDB XpandFeatureCenterExceptionHandling
#DropDB XpandFeatureCenterMultipleDataStore
#DropDB XpandFeatureCenterWorldCreator
#Application FeatureCenter.Win
;#Application FeatureCenter.Web
 
*ScreenCapture
*FillForm
 User Name = Admin
 Password = Admin
*Action Log On

*Action Navigation(WorldCreator.Persistent Assembly Info)
*ProcessRecord 
 Name = MasterDetailDynamicAssembly
!FieldIsVisible
 Fields = Compile Errors 
*SaveAndClose

*ProcessRecord 
 Name = [PersistentAssemblyInfo]
!FieldIsVisible
 Fields = Compile Errors 
*SaveAndClose

