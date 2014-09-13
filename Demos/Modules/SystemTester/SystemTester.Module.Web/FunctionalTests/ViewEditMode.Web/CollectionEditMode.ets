#IncludeFile .\Init.inc
*Navigate ViewEditMode.View
*Action New(View Edit Mode Object)
*FillForm
 Name = View
*SaveAndClose
*Navigate ViewEditMode.View
*ProcessRecord
 Name = View

*CheckTable View Edit Mode Children
 Columns = Child Name

*Action Edit
*CheckTable View Edit Mode Children
 Columns = Child Name
*ActionAvailable View Edit Mode Children.Link