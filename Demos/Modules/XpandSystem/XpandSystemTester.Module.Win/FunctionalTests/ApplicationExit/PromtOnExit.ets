#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

#IncludeFile ..\Login.inc
#IncludeFile DefaultModel.inc
#IncludeFile ..\DefaultNavigation.inc

*ActionAvailable Application Exit
*Action Application Exit(PromptOnExit) 
*HandleDialog
 Respond = No
*ActionAvailable Application Exit

*Action Model Value(Options;MinimizeOnExit;True)
*Action Application Exit(PromptOnExit) 
!ActionAvailable Application Exit
