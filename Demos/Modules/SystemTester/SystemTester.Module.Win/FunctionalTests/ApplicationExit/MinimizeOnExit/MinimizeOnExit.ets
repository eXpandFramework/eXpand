#DropDB SystemTesterEasyTest
#Application SystemTester.Win
*UseModel ..\ApplicationExit
*LogOn
*ScreenCapture
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Refresh
*ActionAvailable Application Exit
*Action Application Exit(MinimizeOnExit) 
!ActionAvailable Application Exit
*ActionAvailable Refresh

*Action Log Off
!ActionAvailable Log Off
*LogOn

*Action Edit Model
!ActionAvailable Edit Model

