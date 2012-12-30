;#DropDB ModelArtifactStateTesterEasyTest

#Application ModelArtifactStateTester.Win
#Application ModelArtifactStateTester.Web

#IfDef ModelArtifactStateTester.Win

*Action About...
 
*Action OK

#EndIf

#IfDef ModelArtifactStateTester.Web

*Action Choose Theme(Black Glass)

#EndIf
