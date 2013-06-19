;#DropDB ConditionalDetailViewsTesterEasyTest

#Application ConditionalDetailViewsTester.Win
#Application ConditionalDetailViewsTester.Web

#IfDef ConditionalDetailViewsTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef ConditionalDetailViewsTester.Web

*Action Choose Theme(Black Glass)

#EndIf
