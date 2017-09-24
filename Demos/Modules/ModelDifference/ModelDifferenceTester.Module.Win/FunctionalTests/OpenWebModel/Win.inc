;#Application ModelDifferenceTester.Win
;*LogOn
#IfDef ModelDifferenceTester.Win
*Navigate Application Settings
*XpandProcessRecord
 Name = ModelDifferenceTester.Win-All
 Persistent Application = ModelDifferenceTester.Win
*SaveAndClose
*XpandProcessRecord
 Name = ModelDifferenceTester.Web-All
 Persistent Application = ModelDifferenceTester.Web
*SaveAndClose
*XpandProcessRecord
 Name = Auto*
 Persistent Application = ModelDifferenceTester.Win
*SaveAndClose
*XpandProcessRecord
 Name = Auto*
 Persistent Application = ModelDifferenceTester.Web
*SaveAndClose
#EndIf