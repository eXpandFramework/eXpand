#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

#IncludeFile ..\Login.inc
#IncludeFile DefaultModel.inc
#IncludeFile ..\DefaultNavigation.inc

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

