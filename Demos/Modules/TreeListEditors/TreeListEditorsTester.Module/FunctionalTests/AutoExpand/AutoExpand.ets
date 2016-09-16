#DropDB TreeListEditorsTesterEasyTest
#Application TreeListEditorsTester.Win
#Application TreeListEditorsTester.Web
*ScreenCapture
*UseModel
*LogOn
*Navigate Auto Expand Object

*Action New
*XpandFillForm
 Name = Root
*Action Save and New
*XpandFillForm
 Name = Child
 Parent = Root
*Action Save and New
*XpandFillForm
 Name = SubChild
 Parent = Child
*SaveAndClose
*Navigate Auto Expand Object
#IfDef TreeListEditorsTester.Win
*DblClick [SubChildWin]
#EndIf
#IfDef TreeListEditorsTester.Web
*DblClick [SubChildWeb]
#EndIf
*XpandCheckFieldValues
 Parent = Child
 Name = SubChild


