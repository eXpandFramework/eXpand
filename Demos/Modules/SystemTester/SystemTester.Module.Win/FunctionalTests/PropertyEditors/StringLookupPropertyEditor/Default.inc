;#IncludeFile .\Init.inc

*Action Navigation(PropertyEditors.StringLookup.Default)
*Action New
*SendKeys t
*SaveAndClose
*Action Navigation(PropertyEditors.StringLookup.Default)
*CheckTable
 Columns = Phone
 Row[0] = text
 Row[1] = text
