;#IncludeFile .\Init.inc

*Action Navigation(PropertyEditors.StringLookup.Mask)
*XpandProcessRecord
 Phone = text
*CheckFieldValues
 Phone = _____
*XpandFillForm
 Phone = 111
*SaveAndClose
*XpandProcessRecord
 Phone = 111
*SaveAndClose
