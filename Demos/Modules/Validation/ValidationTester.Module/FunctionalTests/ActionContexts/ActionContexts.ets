#DropDB ValidationTesterEasyTest
#Application ValidationTester.Win
;#Application ValidationTester.Web
*ScreenCapture
*UseModel
*LogOn
*Navigate Validation Object
*Action New
*XpandFillForm
 Name = test
*Action Save
*Action Refresh
*CheckValidationResult 
 Info = do not refresh test
*Action Ignore
*SaveAndClose
