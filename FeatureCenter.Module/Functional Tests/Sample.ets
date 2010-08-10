;#DropDB FeatureCenterEasyTest

#Application FeatureCenterWin
#Application FeatureCenterWeb

*Action Navigation(User)

*ProcessRecord
 Full Name = ''

#IfDef FeatureCenterWeb
*Action Edit
#EndIf
 
*FillForm
 Full Name = Test User
 
*Action Cancel

#IfDef FeatureCenterWin
*HandleDialog
 Respond = Yes
#EndIf

*CheckFieldValues
 Full Name = ''
