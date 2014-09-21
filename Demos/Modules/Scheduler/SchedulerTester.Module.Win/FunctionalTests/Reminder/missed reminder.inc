;#IncludeFile .\Init.inc
*Navigate Default.Reminder Object
*Action New(Reminder Object)
*FillForm
 Has Reminder = True
 Subject = MissedReminder
*SaveAndClose
*ReopenApplication
*Action Log On
*Navigate Default.Role
*Sleep 5000
*XpandHandleDialog
 Caption = 1 Reminder
 Respond = Dismiss