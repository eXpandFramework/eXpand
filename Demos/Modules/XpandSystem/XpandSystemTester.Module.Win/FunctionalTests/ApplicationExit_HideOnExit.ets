#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

#IncludeFile Login.inc
#IncludeFile ApplicationExit_Model.inc
#IncludeFile DefaultNavigation.inc

*ActionAvailable Application Exit
*Action Application Exit(HideOnExit) 
!ActionAvailable Application Exit

*Action Log Off
!ActionAvailable Log Off
#IncludeFile Login.inc

*Action Edit Model
!ActionAvailable Edit Model


