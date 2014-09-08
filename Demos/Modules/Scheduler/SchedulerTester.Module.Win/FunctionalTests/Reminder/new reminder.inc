;#IncludeFile .\Init.inc
*Navigate Default.Reminder Object
*Action New(Reminder Object)
*FillForm
 Has Reminder = True
 Subject = NewReminder
*SaveAndClose
*Sleep 1000
*HandleDialog
 Caption = 1 Reminder
 Respond = Dismiss
*XpandProcessRecord
 Subject = NewReminder
*CheckFieldValues
 Has Reminder = False
