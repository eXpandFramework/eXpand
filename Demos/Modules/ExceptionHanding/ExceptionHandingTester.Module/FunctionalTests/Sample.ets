;#DropDB ExceptionHandingTesterEasyTest

#Application ExceptionHandingTester.Win
#Application ExceptionHandingTester.Web

#IfDef ExceptionHandingTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef ExceptionHandingTester.Web

*Action Choose Theme(Black Glass)

#EndIf
