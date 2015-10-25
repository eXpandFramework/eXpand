#IncludeFile .\Init.inc

*Action Register User
*XpandFillForm
 User Name = user1
 Password = pass
 Email = user1@mail.com
*Action Register User
*LogOn
*Navigate Email Object
*XpandProcessRecord 
 Subject = User activation

*XpandCheckFieldValues
 From = sender@mail.com
 To = user1@mail.com
 Reply To = mymail@mail.com
 Body = A new user user1 has been created. To activate the account please*

*LogOff
*Action Restore Password
*XpandFillForm
 Email = user1@mail.com
*Action Restore Password

*LogOn
*Navigate Email Object
*XpandProcessRecord 
 Subject = pass forgotten

*XpandCheckFieldValues
 From = sender@mail.com
 To = user1@mail.com
 Reply To = mymail@mail.com
 Body = We created a temporary password *

 

