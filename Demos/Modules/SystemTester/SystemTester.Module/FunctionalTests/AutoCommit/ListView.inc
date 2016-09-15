;#IncludeFile .\Init.inc
*Action Navigation(Auto Commit Object)
*DeleteAllObjects
*Action New
*XpandFillForm
 Name = Tolis
*Action Save
*Action Navigation(Auto Commit Object)

#IfDef SystemTester.Win
*LClick [FirstRecordCheckBoxWin]
*Sleep 400
*SendKeys
 Keys = TAB
*Action Navigation(Auto Commit Object)
#EndIf

#IfDef SystemTester.Web
*ToggleNavigation
*LClick [FirstRecordCheckBoxWeb]
*ToggleNavigation
*Mouse
 MoveMouseTo = [OutsideEditor]
*Sleep 1000
#EndIf

*CheckTable
 Columns = Check
 Row = True
