#DropDB SecurityTesterEasyTest
;#Application SecurityTester.Win
#Application SecurityTester.Web


*XpandFillForm
 User Name = Admin
 Password = ''
 Remember Me = False
*Action Log On

*ChangeUserPassword 1111

*Action Log Off
*FillForm
 Password = 1111
 Remember Me = True
*Action Log On

*ScreenCapture