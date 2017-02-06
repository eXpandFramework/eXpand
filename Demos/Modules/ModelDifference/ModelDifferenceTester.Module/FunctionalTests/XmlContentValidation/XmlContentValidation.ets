#DropDB ModelDifferenceTesterEasyTest
#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*LogOn
*ScreenCapture

;-------set invalid xml ---------------------
*Navigate Application Settings
*XpandProcessRecord
 Name = ModelDifferenceTester*
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = Invalid xml

;-------test validation on focus changes-------
#IfDef ModelDifferenceTester.Win
*LClick [ModelTab]
*Sleep 1000
*XpandCheckValidationResult
 Info = *invalid*
#EndIf
 
;-------test validation on commit changes-------
*Action Save
*XpandCheckValidationResult
 Info = *invalid*

;------set invalid model schema
*FillForm
 Xml Content = <Application><Views></ListView></Views></Application>

;-------test validation on focus changes-------
#IfDef ModelDifferenceTester.Win
*LClick [ModelTab]
*XpandCheckValidationResult
 Info = *ListView*
#EndIf

;-------test validation on commit changes-------
*Action Save
*XpandCheckValidationResult
 Info = *ListView*
