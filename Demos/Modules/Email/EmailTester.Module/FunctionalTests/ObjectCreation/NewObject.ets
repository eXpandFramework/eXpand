#IncludeFile .\Init.inc

;------------------Send to admins-------------------------------
*Navigate Customer
*Action New
*XpandFillForm
 First Name = Tolis
 Last Name = Bekiaris
*SaveAndClose

*Navigate Email Object
*XpandProcessRecord
 Subject = New Customer
*XpandCheckFieldValues
 From = sender@mail.com
 To = admin@mail.com
 Reply To = mymail@mail.com
 Subject = New Customer
 Body = A new customer created with fullname Tolis Bekiaris*

;------------------Send to object instance members-------------------------------
*Navigate Project
*Action New
*XpandFillForm
 Name = test
*Action Members.New
*XpandFillForm
 Email = m1@mail.com
*SaveAndClose
*Action Members.New
*XpandFillForm
 Email = m2@mail.com
*SaveAndClose
*SaveAndClose

*Navigate Email Object
*XpandProcessRecord 
 Subject = Project created

*XpandCheckFieldValues
 From = sender@mail.com
 To =  m1@mail.com, m2@mail.com
 Reply To = mymail@mail.com
 Body = We created a new project (test)*
