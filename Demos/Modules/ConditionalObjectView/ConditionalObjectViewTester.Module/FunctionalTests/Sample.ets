;#DropDB ConditionalObjectViewTesterEasyTest

#Application ConditionalObjectViewTester.Win
#Application ConditionalObjectViewTester.Web

#IfDef ConditionalObjectViewTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef ConditionalObjectViewTester.Web

*Action Choose Theme(Black Glass)

#EndIf
