#DropDB IOTesterEasyTest
#DropDB NorthWind
#Application IOTester.Win
;#Application IOTester.Web
*ScreenCapture
*LogOn

;----------------------Import--------------------------------
*Action Navigation(WorldCreator.Persistent Assembly Info)
*ProcessRecord 
 Name = [PersistentAssemblyInfo]
!FieldIsVisible
 Fields = Compile Errors 

*ReopenApplication
*LogOn
*Action Navigation(WorldCreator.[PersistentAssemblyInfo].BOModel.Categories.Categories)
*Action Navigation(WorldCreator.[PersistentAssemblyInfo].BOModel.Categories.CustomerCustomerDemo)

;----------------------export-------------------------------
*Action Navigation(ImportExport.Serialization Configuration Group)
*Action New(Serialization Configuration Group)
*FillForm
 Name = PersistentAssemblyInfo
*Action Serialization Configurations.New
*FillForm
 Type To Serialize = Persistent Assembly Info
*SaveAndClose
*SaveAndClose

*Action Navigation(WorldCreator.Persistent Assembly Info)
*SelectRecords
 Row = [PersistentAssemblyInfo]
 Columns = Name
*Action IO(Export)
*SelectRecords Serialization Configuration Group
 Columns = Name
 Row = PersistentAssemblyInfo
*Action OK
*SaveFileDialog [PersistentAssemblyInfo].xml
*Sleep 1000
*OptionalAction Cancel
*OptionalAction Save and Close
*ExecuteTableAction
 SelectAll = True
*Action Delete
*HandleDialog
 Respond = Yes
*ReopenApplication
*LogOn
*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog [PersistentAssemblyInfo].xml
*Action OK
*Action Refresh
*ReopenApplication
*LogOn

*Action Navigation(WorldCreator.Persistent Assembly Info)
*ProcessRecord 
 Name = [PersistentAssemblyInfo]
!FieldIsVisible
 Fields = Compile Errors 

*Action Navigation(WorldCreator.[PersistentAssemblyInfo].BOModel.Categories.Categories)
*Action Navigation(WorldCreator.[PersistentAssemblyInfo].BOModel.Categories.CustomerCustomerDemo)


