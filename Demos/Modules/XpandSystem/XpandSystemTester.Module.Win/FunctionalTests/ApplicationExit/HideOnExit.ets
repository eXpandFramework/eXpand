#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

*UseModel
#IncludeFile ..\Login.inc
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Application Exit
*Action Application Exit(HideOnExit) 
!ActionAvailable Application Exit

*Action Log Off
!ActionAvailable Log Off
#IncludeFile ..\Login.inc

*Action Edit Model
!ActionAvailable Edit Model


