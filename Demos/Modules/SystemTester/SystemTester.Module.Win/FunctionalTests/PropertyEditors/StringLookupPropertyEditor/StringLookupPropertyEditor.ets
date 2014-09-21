#IncludeFile .\Init.inc

*Action New(String Lookup Property Editor Object)
*FillForm
 Phone = text
*Action Save and New
*SendKeys t
*Action Save
*Action Navigation(PropertyEditors.StringLookup.Default)
*CheckTable
 Columns = Phone
 Row[0] = text
 Row[1] = text
 
*Action Navigation(PropertyEditors.StringLookup.Mask)
*XpandProcessRecord
 Phone = text
*CheckFieldValues
 Phone = _____
*XpandFillForm
 Phone = 111
*Action Save
