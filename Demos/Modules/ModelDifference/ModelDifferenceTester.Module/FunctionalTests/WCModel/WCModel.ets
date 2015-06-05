#Application ModelDifferenceTester.Web
#Application ModelDifferenceTester.Win
#IfDef ModelDifferenceTester.Web
*SqlDropDB 
 DBName = ModelDifferenceTesterEasyTest
#EndIf

*LogOn
*ScreenCapture

#IfDef ModelDifferenceTester.Web
#IncludeFile .\CreateWCAssembly.inc
*Navigate Application Difference
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
*Navigate Application Difference
*XpandProcessRecord 
 Name = ModelDifferenceTester.Web
 Persistent Application = ModelDifferenceTester.Web
*Action Xml Content
*XpandCheckFieldValues
 Xml Content = *Changed*
#EndIf