;#DropDB AuditTrailTesterEasyTest

#Application AuditTrailTester.Win
#Application AuditTrailTester.Web

#IfDef AuditTrailTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef AuditTrailTester.Web

*Action Choose Theme(Black Glass)

#EndIf
