#DropDB SystemTesterEasyTest
#Application SystemTester.Win
;#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture
*Action Navigation(Default.Appearance Rule Object)

*Action New(Appearance Rule Object)
*XpandFillForm
 Name = Hide name in detailview
 Context = Detail view
 Declaring Type = Appearance Rule Object
 Target Items = Name
 Enabled = False
*SaveAndClose
*Action Navigation(Default.Appearance Rule Object)
*XpandProcessRecord 
 Name = Hide name in detailview
!XpandFillForm
 Name = Change

 