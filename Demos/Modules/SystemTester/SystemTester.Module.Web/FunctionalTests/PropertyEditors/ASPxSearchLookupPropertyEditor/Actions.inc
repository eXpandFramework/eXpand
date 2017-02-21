;#IncludeFile .\Init.inc
*SetWebMaxWaitTimeOut 600000
*Navigate ASPx Search Lookup Property Editor Object
*DeleteAllObjects
*Action New
*ToggleNavigation
*Mouse LeftButtonClick
 MoveMouseTo = [NewAction]
*XpandFillForm
 Name = Bekiaris
*SaveAndClose
*ToggleNavigation
*SaveAndClose
*XpandProcessRecord
 ASPx Search Lookup Property Editor Lookup = Bekiaris

*Mouse LeftButtonClick
 MoveMouseTo = [ClearAction]
 ToggleNavigation = True
*SaveAndClose
*CheckValidationResult
 Message = Problems were detected during data validation*

*ToggleNavigation
*Mouse LeftButtonClick
 MoveMouseTo = [SearchAction]
*Sleep 1000
*HandleDialog
 Caption = ASPx Search Lookup Property Editor Lookup Object
*Sleep 1000
*ProcessRecord
 Name = Bekiaris
*ToggleNavigation
*XpandCheckFieldValues
 ASPx Search Lookup Property Editor Lookup = Bekiaris
*SaveAndClose
!ActionAvailable Save
*Navigate ASPx Search Lookup Property Editor Object
*CheckTable
 Columns = ASPx Search Lookup Property Editor Lookup
 Row = Bekiaris


