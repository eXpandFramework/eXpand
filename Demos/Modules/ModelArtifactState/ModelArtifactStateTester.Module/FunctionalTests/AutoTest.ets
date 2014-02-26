#DropDB ModelArtifactStateTesterEasyTest

#Application ModelArtifactStateTester.Win
#Application ModelArtifactStateTester.Web

#IfDef ModelArtifactStateTester.Win
*FillForm
 User Name = Admin
*Action Log On
#EndIf

*AutoTest

