#DropDB SecurityTesterEasyTest
#Application SecurityTester.Win
;#Application SecurityTester.Web

*XpandFillForm
 User Name = Admin
 Password = ''
 Remember Me = False
*XpandCompareScreenshot LogonParameters.png
 AdditionalCommands = False
 WebMaskRectangle = 268x120;505x183
 WinMaskRectangle = 18x36;399x155
 ValidDiffPercentage = 5