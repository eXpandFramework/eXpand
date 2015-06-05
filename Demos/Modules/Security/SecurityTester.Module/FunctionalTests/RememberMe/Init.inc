#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
;#Application SecurityTester.Web
*UseModel

*XpandFillForm
 User Name = Admin
 Password = ''
 Remember Me = False
*LogOn

*ChangeUserPassword 1111

*Action Log Off
*FillForm
 Password = 1111
 Remember Me = True
*LogOn

*ScreenCapture