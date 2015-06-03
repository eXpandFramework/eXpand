#IncludeFile .\Init.inc

*Action Navigation(PropertyEditors.FilterableEnumPropertyEditorObject)
*Action New

*XpandFillForm
 Filtered Status = White
*XpandCheckFieldValues
 Filtered Status = White
*XpandFillForm
 Filtered Status = Blue
*XpandCheckFieldValues
 Filtered Status = Blue


*XpandFillForm
 Filtered Status = Green
 Throw = False
*XpandCheckFieldValues
 Filtered Status = Blue
*XpandFillForm
 Filtered Status = Red
 Throw = False
*XpandCheckFieldValues
 Filtered Status = Blue

*XpandFillForm
  Switch = True

*XpandFillForm
 Filtered Status = Green
*XpandCheckFieldValues
 Filtered Status = Green
*XpandFillForm
 Filtered Status = Red
*XpandCheckFieldValues
 Filtered Status = Red


*XpandFillForm
 Filtered Status = White
 Throw = False
*XpandCheckFieldValues
 Filtered Status = Red
*XpandFillForm
 Filtered Status = Blue
 Throw = False
*XpandCheckFieldValues
 Filtered Status = Red