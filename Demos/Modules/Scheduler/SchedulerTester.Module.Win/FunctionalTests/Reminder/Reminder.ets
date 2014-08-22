#DropDB SchedulerTesterEasyTest

#Application SchedulerTester.Win

*UseModel

*Action New(Reminder Object)
*FillForm
 Has Reminder = True
*SaveAndClose
*Sleep 5000
*HandleDialog
 Caption = 1 Reminder
