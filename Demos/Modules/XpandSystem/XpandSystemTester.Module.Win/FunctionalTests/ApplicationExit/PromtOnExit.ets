#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

*UseModel
#IncludeFile ..\Login.inc
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Application Exit
*Action Application Exit(PromptOnExit) 
*HandleDialog
 Respond = No
*ActionAvailable Application Exit

*Action Application Exit(PromptOnExit) 
!ActionAvailable Application Exit
