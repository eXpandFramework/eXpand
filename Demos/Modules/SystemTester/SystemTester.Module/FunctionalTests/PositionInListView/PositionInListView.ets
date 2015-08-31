#DropDB SystemTesterEasyTest
#Application SystemTester.Win
;#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture

*Action Navigation(Default.Position In List View Object)
*ActionVisible Up
*ActionVisible Down
!ActionAvailable Up
!ActionAvailable Down

*Action New
*XpandFillForm
 Name = A
*SaveAndClose
!ActionAvailable Up
!ActionAvailable Down
*Action New
*XpandFillForm
 Name = B
*SaveAndClose
!ActionAvailable Up
*ActionAvailable Down
*CheckTable
 Columns = Name
 Row[0] = A
 Row[1] = B

!ActionAvailable Up
*Action Down
*CheckTable
 Columns = Name
 Row[1] = A
 Row[0] = B

!ActionAvailable Down
*Action Up
*CheckTable
 Columns = Name
 Row[0] = A
 Row[1] = B
