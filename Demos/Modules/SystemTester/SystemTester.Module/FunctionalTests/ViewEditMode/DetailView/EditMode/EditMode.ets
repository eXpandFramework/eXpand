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
 Columns = Child Name
*Navigate ViewEditMode.Edit

*SelectRecords
 Columns = Name
 Row = Edit
*Action View Edit Mode
*FillForm
 Name = Edit

