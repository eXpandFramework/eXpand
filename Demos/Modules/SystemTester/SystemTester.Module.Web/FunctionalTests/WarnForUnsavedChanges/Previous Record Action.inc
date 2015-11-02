;#IncludeFile .\Init.inc

*Action New
*XpandFillForm
 Name = Previous
*Action Save
*Action New
*XpandFillForm
 Name = Previous2
*SaveAndClose
*Navigate Warn For Unsaved Changes Object
*XpandProcessRecord
 Name = Previous2
*XpandFillForm
 Name = Previous Changed
*Action Previous Record
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Save
*Navigate Warn For Unsaved Changes Object
*Navigate Warn For Unsaved Changes Object