#IncludeFile .\Init.inc

*LogOn Admin
*Action Navigation(Default.Role)
*XpandProcessRecord 
 Name = User
*XpandFillForm
 Modify Layout = True
*SaveAndClose
*Action Log Off
*LogOn User
*Navigate Overall Customization Allowed Object
*Action New
*XpandFillForm
 Field = Test
*Mouse RightButtonClick
 MoveMouseTo = [EmptyAreaOfDetailView]
*Sleep 1000
*SendKeys
 Keys = Down;Return
*Sleep 2000

*Mouse LeftButtonClick
 MoveMouseTo = [FieldArea]
*Sleep 1000
*SendKeys
 Keys = Delete
*Sleep 500
*SaveAndClose
*XpandProcessRecord
 Field = Test
!XpandFillForm
 Field = ModifyValue