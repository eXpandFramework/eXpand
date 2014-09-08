;#IncludeFile .\Init.inc
*Navigate Default.Reminder Object
*Action New(Reminder Object)
*FillForm
 Has Reminder = True
 Subject = MissedReminder
*SaveAndClose
*ReopenApplication
*Navigate Default.Reminder Object
*XpandHandleDialog
 Caption = 1 Reminder
 Respond = Dismiss