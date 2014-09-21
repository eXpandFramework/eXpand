;#IncludeFile .\Init.inc
;Create new reminder
*Navigate Default.Reminder Object
*Action New(Reminder Object)
*FillForm
 Has Reminder = True
 Subject = UpdateReminder
*SaveAndClose
*Sleep 1000
*HandleDialog
 Caption = 1 Reminder
*XpandProcessRecord
 Subject = UpdateReminder
*CheckFieldValues
 Has Reminder = True

;Remove the reminder
*XpandFillForm
 Has Reminder = False
*SaveAndClose

;Check if removed
*ReopenApplication
*Action Log On
*Navigate Default.Reminder Object
!XpandHandleDialog
 Caption = 1 Reminder

