#IncludeFile .\Init.inc
*Action Register User
*FillForm
 User Name = NewUser
 Email = newuser@mail.com
 Password = 123
*Action Register User

*LogOn NewUser (123)
*Navigate My Details
*XpandCheckFieldValues
 User Name = NewUser
 Email = newuser@mail.com
*ProcessRecord Roles
 Name = User
*SaveAndClose
*ProcessRecord Roles
 Name = Default
*SaveAndClose