#IncludeFile .\Init.inc
 
;------------------Send to admins-------------------------------
*Navigate Email Task
*Action New
*Action Save
*XpandFillForm
 Subject = emailfromaction
*Action Send Email
*SaveAndClose


*Navigate Email Object
*CheckTable Email Object
 RowCount = 1 
*XpandProcessRecord
 Subject = email from action
*XpandCheckFieldValues
 From = sender@mail.com
 To = admin@mail.com
 Reply To = mymail@mail.com
 Subject = email from action
 Body = Email send for the task with subject (emailfromaction)*
