#IncludeFile .\Init.inc
*Navigate ViewEditMode.View
*Action New(View Edit Mode Object)
*FillForm
 Name = View
*SaveAndClose
*Navigate ViewEditMode.View
*ProcessRecord
 Name = View

!FillForm
 Name = View
*Action Edit
*FillForm
 Name = View

*Navigate ViewEditMode.View

*SelectRecords
 Columns = Name
 Row = View
*Action View Edit Mode
!FillForm
 Name = View
