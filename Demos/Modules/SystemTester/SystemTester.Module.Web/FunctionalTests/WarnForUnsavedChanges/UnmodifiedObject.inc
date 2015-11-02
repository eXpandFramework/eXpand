;#IncludeFile .\Init.inc

*Navigate Warn For Unsaved Changes Object
*Action New
*XpandFillForm
 Name = UnmodifiedObject
*SaveAndClose
*XpandProcessRecord 
 Name = UnmodifiedObject
*Action Cancel
*SelectRecords
 Columns = Name
 Row = UnmodifiedObject
