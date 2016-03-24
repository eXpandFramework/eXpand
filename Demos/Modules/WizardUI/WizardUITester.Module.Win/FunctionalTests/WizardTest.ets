#DropDB WizardUITesterEasyTest
#Application WizardUITester.Win
*ScreenCapture
*UseModel
*LogOn

*Navigate Wizard Object
*Action New

;----Check validation-------
*HandleDialog
 Caption = Wizard Object
 Respond = Next >
!HandleDialog
 Caption = Wizard Object
 Respond = Finish

;----change value to fit criteria
*XpandFillForm
 Page 1 = 0 
*Mouse LeftButtonClick
 MoveMouseTo = [Page1UpArrow]
;---move to next page------
*HandleDialog
 Caption = Wizard Object
 Respond = Next >
;-----check that it is the final page----
!HandleDialog
 Caption = Wizard Object
 Respond = Next >

;---check the reulerequeired on page2 field-----
*HandleDialog
 Caption = Wizard Object
 Respond = Finish
*HandleDialog
 Caption = Wizard Object
 Respond = Finish

;-----change the Page 2 value------
*Mouse LeftButtonClick
 MoveMouseTo = [Page2Field]
*SendKeys
 Keys = v;a;l;u;e

*HandleDialog
 Caption = Wizard Object
 Respond = Finish

*CheckTable
 Columns = Page 1, Page 2
 Row[0] = 1, value

