#IncludeFile .\Init.inc
*Navigate Warn For Unsaved Changes Object
*Action New
*XpandFillForm
 Name = Test

;-----Check Cancel Action ------------
*Action Cancel
*CheckValidationResult
 Message = 'Please, click Cancel again to cancel your changes.'
*Action Cancel

*Navigate Warn For Unsaved Changes Object
*Action New
*XpandFillForm
 Name = Test

;-----Check New Action ------------
*Action New
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action New
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Cancel

*Navigate Warn For Unsaved Changes Object
*Action New
*XpandFillForm
 Name = Test
*SaveAndClose
*Action New
*XpandFillForm
 Name = Test2
*SaveAndClose
*Navigate Warn For Unsaved Changes Object
*XpandProcessRecord 
 Name = Test2
*XpandFillForm
 Name = Test3

;-----Check Delete Action ------------
*Action Delete
*HandleDialog 
 Respond = OK
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'

;-----Check Refresh Action ------------
*Action Refresh
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
 
;-----Check Previous Record Action ------------
*Action Previous Record
*CheckValidationResult
 Message = 'You have unsaved changes. Please, Save or Cancel them.'
*Action Save

;-----Check unmodified object ------------
*Navigate Warn For Unsaved Changes Object
*XpandProcessRecord 
 Name = Test
*Action Cancel
*SelectRecords
 Columns = Name
 Row = Test
 


