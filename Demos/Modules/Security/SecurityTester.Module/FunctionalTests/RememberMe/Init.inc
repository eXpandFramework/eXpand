#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
;#Application SecurityTester.Web

*LogOn Admin ()

*ChangeUserPassword 123

*Action Log Off
*XpandCheckFieldValues
 Password = ''
*FillForm
 Password = 123
 Remember Me = True
*LogOn

*ScreenCapture