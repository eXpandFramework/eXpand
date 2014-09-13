#IncludeFile .\Init.inc
*Navigate ViewEditMode.Edit
*Action New(View Edit Mode Object)
*FillForm
 Name = Edit
*SaveAndClose
*Navigate ViewEditMode.Edit
*ProcessRecord
 Name = Edit

*FillForm
 Name = Edit
*CheckTable View Edit Mode Children
 Columns = View Edit Mode Object
*Navigate ViewEditMode.Edit

*SelectRecords
 Columns = Name
 Row = View
*Action View Edit Mode
*FillForm
 Name = View

