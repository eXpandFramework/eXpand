#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

*UseModel
#IncludeFile ..\Login.inc
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Refresh
*ActionAvailable Application Exit
*Action Application Exit(MinimizeOnExit) 
!ActionAvailable Application Exit
*ActionAvailable Refresh

*Action Log Off
!ActionAvailable Log Off
#IncludeFile ..\Login.inc

*Action Edit Model
!ActionAvailable Edit Model

