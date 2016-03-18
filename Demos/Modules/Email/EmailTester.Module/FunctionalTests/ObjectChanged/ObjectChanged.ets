#IncludeFile .\Init.inc
 
;------------------Send to admins-------------------------------
*Navigate Email Task
*Action New
*XpandFillForm
 Subject = test
*Action Save
*XpandFillForm
 Status = In progress
*SaveAndClose

*Navigate Email Object
*CheckTable 
 RowCount = 1 
*XpandProcessRecord
 Subject = TaskStatusChanged
*XpandCheckFieldValues
 From = sender@mail.com
 To = admin@mail.com
 Reply To = mymail@mail.com
 Subject = TaskStatusChanged
 Body = The Status of (test) changed to (InProgress)*
