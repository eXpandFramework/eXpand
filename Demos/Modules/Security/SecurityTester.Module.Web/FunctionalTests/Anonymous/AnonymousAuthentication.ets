#IncludeFile .\Init.inc

*Action Login
*XpandFillForm
 User Name = Admin
*Action OK
*CreatePermission Anonymous Object
 EditRole = Anonymous
*Action Log Off
*Navigate Anonymous Object