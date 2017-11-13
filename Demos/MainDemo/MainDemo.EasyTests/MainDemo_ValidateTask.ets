#DropDB MainDemo
#Application MainDemoWin
#Application MainDemoWeb

*FillForm
 User Name = Sam
*Action Log On

*Action Navigation(Position)

*Action New

*Action Save and Close

*CheckValidationResult
 Message = Problems were detected during data validation. Please read the information below to understand what the issues are and how you can correct them.
 Info = "Title" must not be empty.
*OptionalAction Close

*FillForm
 Title = TechWriter
*Action Save and Close

!HandleDialog
 Message = ?*