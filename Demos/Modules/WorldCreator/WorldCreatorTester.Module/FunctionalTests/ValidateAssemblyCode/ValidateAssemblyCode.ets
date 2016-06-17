#DropDB WorldCreatorTesterEasyTest
#DropDB WorldCreatorTesterCreator
#DropDB WCDBMapperTester
#Application WorldCreatorTester.Win
#Application WorldCreatorTester.Web
*LogOn
*ScreenCapture

;----------assign invalid template-----------------
*Action Navigation(WorldCreator.Persistent Assembly Info)
*XpandProcessRecord 
 Name = OneToMany
 Action = Edit
*Action Persistent Class Infos
*XpandProcessRecord Persistent Class Infos
 Name = Contributor
 Action = Edit
*Action Code
*XpandFillForm
 Template Code = invalid
*SaveAndClose
*SaveAndClose

*CheckValidationResult
 Info = *Compile*
