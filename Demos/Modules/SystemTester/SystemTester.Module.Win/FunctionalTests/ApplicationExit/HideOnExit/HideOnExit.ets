#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel ..\ApplicationExit
*LogOn
*ScreenCapture
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Application Exit
*Action Application Exit(HideOnExit) 
!ActionAvailable Application Exit
*Sleep 1000
*Action Log Off
!ActionAvailable Log Off
*LogOn

*Action Edit Model
!ActionAvailable Edit Model


