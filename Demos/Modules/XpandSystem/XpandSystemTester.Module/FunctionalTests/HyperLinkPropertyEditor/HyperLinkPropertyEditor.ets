#DropDB XpandSystemTesterEasyTest

#Application XpandSystemTester.Web
#Application XpandSystemTester.Win

#IncludeFile ..\Login.inc
*Action Navigation(Default.Hyper Link Object)

;Can save without notification a valid url
*Action New(Hyper Link Object)
*FillForm
 Url = http://www.expandFramework.com
*Action Save and Close
*Action Navigation(Default.Hyper Link Object)
*CheckTable Hyper Link Object
 Columns = Url
 Row[0] = http://www.expandFramework.com

;Does not save an invalid url and notify with an icon
#IfDef XpandSystemTester.Web
*ProcessRecord
 Url = http://www.expandFramework.com
 Action = Edit
#EndIf
#IfDef XpandSystemTester.Win
*ProcessRecord
 Url = http://www.expandFramework.com
*SendKeys aaaa
#EndIf
*SetActiveWindowSize 1024x768
*Sleep 10000
#IfDef XpandSystemTester.Web
*FillForm
 Url = aaaa
#EndIf
*Action Save and Close
#IfDef XpandSystemTester.Web
*XpandCompareScreenshot HyperLinkPropertyEditor.Web.png
 Mask = WebRootDetailView_Mask.png.pic
#EndIf
#IfDef XpandSystemTester.Win
*XpandCompareScreenshot HyperLinkPropertyEditor.Win.png
 Mask = WinDetailView_Mask.png.pic
#EndIf
