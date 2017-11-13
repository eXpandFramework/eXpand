#DropDB MainDemo
#Application MainDemoWin
#Application MainDemoWeb

*FillForm
 User Name = Sam
*Action Log On

*Action Navigation(Task)
*Action New()

*FillForm
 Subject = Prepare financial statements for presentation to boards of directors
 Priority = Normal
 Start Date = 1/11/2006
 Due Date = 1/12/2006
*Action Save and Close

*Action Navigation(Contact)
*ProcessRecord
 Last Name = Tellitson

*OptionalAction Edit

*Action Tasks
*Action Tasks.Link

*ProcessRecord
 Subject = Prepare financial statements for presentation to boards of directors
*CheckTable Tasks
 Columns = Subject
 Row = Prepare financial statements for presentation to boards of directors 

#IfDef MainDemoWeb
!ActionAvailable Tasks.Unlink
#EndIf

*Action Save and Close

*Action Navigation(Task)

*ProcessRecord
 Subject = Prepare financial statements for presentation to boards of directors
*CheckTable Contacts
 Columns = Full Name 
 Row = Tellitson Mary
 
*OptionalAction Save and Close

*Action Navigation(Contact)
*ProcessRecord
 Last Name = Tellitson

*OptionalAction Edit

*Action Tasks

*ProcessRecord Tasks
 Subject = Prepare financial statements for presentation to boards of directors
 Action = Unlink


*HandleDialog
 Respond = Yes

!ProcessRecord Tasks
 Subject = Prepare financial statements for presentation to boards of directors

*Action Save and Close

*Action Navigation(Task)
*ProcessRecord
 Subject = Prepare financial statements for presentation to boards of directors
!ProcessRecord
 Last Name = Tellitson
 