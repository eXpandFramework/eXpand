#DropDB MainDemo
#Application MainDemoWin
#Application MainDemoWeb

*FillForm
 User Name = Sam
*Action Log On

*Action Navigation(Note)

*OptionalAction ObjectsCreation.New

*FillForm
 Text = First note
*Action Save and Close

*OptionalAction ObjectsCreation.New

*FillForm
 Text = Second note
*Action Save and Close

*OptionalAction ObjectsCreation.New

*FillForm
 Text = Third note
*Action Save and Close

*Action Navigation(Task)

*OptionalAction ObjectsCreation.New

*FillForm
 Description = Existent text

*Action Show Notes
*ProcessRecord 
 Text = First note

*CheckFieldValues
 Description[0] = Existent text
 Description[1] = First note

*FillForm
 Description = New text

*Action Show Notes
*ProcessRecord 
 Text = Second note

*CheckFieldValues
 Description[0] = New text
 Description[1] = Second note

*OptionalAction Cancel

#IfDef MainDemoWin
*HandleDialog
 Respond = Yes
#EndIf
