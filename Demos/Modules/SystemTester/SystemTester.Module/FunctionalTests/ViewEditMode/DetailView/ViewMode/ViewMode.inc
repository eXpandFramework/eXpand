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
!ActionVisible Save
!ActionVisible SaveAndClose
!ActionVisible Cancel
*Action Edit
*ActionVisible Save
*ActionVisible Cancel
*FillForm
 Name = View

*Navigate ViewEditMode.View

*SelectRecords
 Columns = Name
 Row = View
*Action View Edit Mode
!FillForm
 Name = View