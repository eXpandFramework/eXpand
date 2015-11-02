;#IncludeFile .\Init.inc

*Action New
*XpandFillForm
 Name = Delete
*SaveAndClose
*XpandProcessRecord 
 Name = Delete
*XpandFillForm
 Name = To be Delete 
*Action Delete
*HandleDialog 
 Respond = OK
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Cancel
*Navigate Warn For Unsaved Changes Object
*Navigate Warn For Unsaved Changes Object
