;#IncludeFile .\Init.inc

*Action New
*Action New
*Navigate Warn For Unsaved Changes Object

*Navigate Warn For Unsaved Changes Object
*Action New
*XpandFillForm
 Name = New
*SaveAndClose
*Navigate Warn For Unsaved Changes Object
*XpandProcessRecord
 Name = New
*XpandFillForm
 Name = New Changed
*Action New
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Cancel
*Navigate Warn For Unsaved Changes Object
*Navigate Warn For Unsaved Changes Object
