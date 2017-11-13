#DropDB MainDemo
#Application MainDemoWin
#Application MainDemoWinRibbon

*FillForm
 User Name = Sam
*Action Log On

*Action Navigation(Task)
*Action New
*FillForm
 Subject = New task  
*Action Save and Close

*CheckTable
 Columns = Subject
 Row = Fix breakfast
 Row = New task
 Row = Review reports

*Action Find Task by Subject(Fix)
*CheckFieldValues
 Subject = Fix breakfast

