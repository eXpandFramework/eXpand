#DropDB WorldCreatorTesterEasyTest
#DropDB WorldCreatorTesterCreator
#DropDB WCDBMapperTester
#Application WorldCreatorTester.Win
#Application WorldCreatorTester.Web
*LogOn
*ScreenCapture

;---------------create member------------------
*Action Navigation(WorldCreator.Extended Core Type Member Info)
*Action New(Extended Core Type Member Info)
*XpandFillForm
 Data Type = String
 Owner = User
 Name = Test
*SaveAndClose


;---------------test member----------------------
*ReopenApplication
*LogOn
*Navigate User

*XpandProcessRecord 
 User Name = user
 Action = Edit
*XpandFillForm
 Test = test
*SaveAndClose
*XpandProcessRecord 
 User Name = user
*CheckFieldValues
 Test = test