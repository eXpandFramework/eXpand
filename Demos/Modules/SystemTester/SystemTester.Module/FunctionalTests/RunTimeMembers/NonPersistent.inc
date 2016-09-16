;#IncludeFile .\Init.inc
*Navigate RuntimeMembers.RunTimeMembers
*XpandProcessRecord
 Full Name = Apostolis Bekiaris
 Action = Edit
*XpandFillForm
 Non Persistent = True
*SaveAndClose
*XpandProcessRecord
 Full Name = Apostolis Bekiaris
 Action = Edit
#IfDef SystemTester.Win
*CheckFieldValues
 Non Persistent = Indeterminate
#EndIf
#IfDef SystemTester.Web
*CheckFieldValues
 Non Persistent = False
#EndIf
