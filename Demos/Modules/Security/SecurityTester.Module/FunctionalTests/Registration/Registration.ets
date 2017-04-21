#IncludeFile .\Init.inc
*Action Register User
#IfDef SecurityTester.Web
*Action Cancel
*Action Register User
#EndIf
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
*CheckTable Roles
 Columns = Name
 Row[0] = Protected Content
 Row[1] = Protected Content
*LogOff
*LogOn Admin
*Navigate User
*XpandProcessRecord 
 User Name = NewUser
*ProcessRecord Roles
 Name = User
*SaveAndClose
*ProcessRecord Roles
 Name = Default
*SaveAndClose