#DropDB SystemTesterEasyTest
#Application SystemTester.Web
#Application SystemTester.Win
*UseModel
*LogOn
*ScreenCapture


*Navigate Track Modified Properties Object
*Action New
*XpandFillForm
 Name = anyName
*Action Save
*Action Modify
*XpandFillForm
 Name = anyName2
*Save
*HandleDialog
 Respond = Merge
*CheckFieldValues
 Name 2 = modify
