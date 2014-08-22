#DropDB SystemTesterEasyTest
#Application SystemTester.Win

*UseModel
*Action Log On
*Action Navigation(Default.Application Exit Object)

*ActionAvailable Application Exit
*Action Application Exit(PromptOnExit) 
*HandleDialog
 Respond = No
*ActionAvailable Application Exit

*Action Application Exit(PromptOnExit) 
!ActionAvailable Application Exit
