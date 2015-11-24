#IncludeFile .\Init.inc

;-------------login -------------
*Navigate Anonymous Object
*Action Login
*LogOn User
*Action Edit
*CheckFieldValues
 User Name = user
*Action Log Off
*Navigate Anonymous Object
*Action Login

;-------Registration-------
*Action Register User
*FillForm
 User Name = NewUser
 Email = newuser@mail.com
 Password = 123
*Action Register User

*LogOn NewUser (123)
*Navigate My Details

