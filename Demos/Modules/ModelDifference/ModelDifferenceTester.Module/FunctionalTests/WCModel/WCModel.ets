#Application ModelDifferenceTester.Web
#Application ModelDifferenceTester.Win
*SetWebMaxWaitTimeOut 60000

*LogOn
*ScreenCapture

#IfDef ModelDifferenceTester.Web

*Action Navigation(WorldCreator.Persistent Assembly Info)
*DeleteAllObjects
*Action New
*FillForm
 Name = TestAssembly
*Action Persistent Class Infos
*Action Persistent Class Infos.New
*FillForm
 Name = TestClass
*Action Own Members
*Action Own Members.New(Core)
*FillForm
 Data Type = String
 Name = TestProperty
*SaveAndClose
*SaveAndClose
*SaveAndClose

*Navigate Application Settings
*XpandProcessRecord 
 Name = ModelDifferenceTester.Web
 Persistent Application = ModelDifferenceTester.Web
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = <Application><BOModel><Class Name="TestAssembly.TestClass"><OwnMembers><Member Name="TestProperty" Caption="Caption Changed" /></OwnMembers></Class></BOModel></Application>
*SaveAndClose
#EndIf

#IfDef ModelDifferenceTester.Win
*Navigate Application Settings
*XpandProcessRecord 
 Name = ModelDifferenceTester.Web
 Persistent Application = ModelDifferenceTester.Web
*Action Xml Content
*XpandCheckFieldValues
 Xml Content = *Changed*
*Action Show Unusable Data
*XpandHandleDialog
 Caption = There are no unusable nodes
#EndIf