#IncludeFile .\Init.inc
;---------Create User from THe NewUserActionUpdater-----------------------

*LogOn
*Navigate New User Activation Object
*Action Activate New User
*LogOn
*Navigate User
*ProcessRecord 
 User Name = NewUserActivation
*CheckFieldValues 
 Is Active = True