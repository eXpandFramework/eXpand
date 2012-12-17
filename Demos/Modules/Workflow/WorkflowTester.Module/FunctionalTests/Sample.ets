;#DropDB WorkflowTesterEasyTest

#Application WorkflowTester.Win
#Application WorkflowTester.Web

#IfDef WorkflowTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef WorkflowTester.Web

*Action Choose Theme(Black Glass)

#EndIf
