;#IncludeFile .\Init.inc

*Action New
*XpandFillForm
 Name = Refresh
*Action Save
*XpandFillForm
 Name = Refresh Changed
*Action Refresh
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Cancel
*Navigate Warn For Unsaved Changes Object
*Navigate Warn For Unsaved Changes Object
