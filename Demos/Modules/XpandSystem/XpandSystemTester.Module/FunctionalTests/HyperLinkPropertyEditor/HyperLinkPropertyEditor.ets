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
*ProcessRecord
 Url = http://www.expandFramework.com
 Action = Edit
*XpandFillForm
 Url = aaaa
 SendKeysX = True

*SetActiveWindowSize 1024x768
*Action Save and Close
*XpandCompareScreenshot HyperLinkPropertyEditor.png
 Mask = /regX/MainView.png.pic

