#DropDB DashboardTesterEasyTest
#Application DashboardTester.Win
#Application DashboardTester.Web
*UseModel
*LogOn

*Action Navigation(Default.MasterDetail)
*Action DetailView.New
*XpandFillForm
 DetailView.Name = First
#IfDef DashboardTester.Web
*Action DetailView.Save
#EndIf
#IfDef DashboardTester.Win
*Action DetailView.Save.Save
#EndIf
*Action DetailView.New
*XpandFillForm
 DetailView.Name = Second
#IfDef DashboardTester.Web
*Action DetailView.Save
#EndIf
#IfDef DashboardTester.Win
*Action DetailView.Save.Save
#EndIf
*ProcessRecord Master Detail Object
 Name = First
*XpandCheckFieldValues
 DetailView.Name = First

