#IncludeFile .\Init.inc

*Action New(Active Document View)
*CheckFieldValues
 Name = active
*Action Save
*Action New(Active Document View)
*CheckFieldValues
 Name = active
*Action Save
*Navigate Active Document View
*CheckTable 
 RowCount = 2 
 Columns = Name
 Row[0] = active
 Row[1] = inactive