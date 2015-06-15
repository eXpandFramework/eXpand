#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
#Application SecurityTester.Web
*UseModel

*LogOn

*ChangeUserPassword 1111

*Action Log Off
*XpandCheckFieldValues
 Password = ''
*FillForm
 Password = 1111
 Remember Me = True
*LogOn

*ScreenCapture