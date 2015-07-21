;#IncludeFile .\Init.inc
*Navigate ASPx Search Lookup Property Editor Object
*Action New
*Mouse LeftButtonClick
 MoveMouseTo = [NewAction]
 ToggleNavigation = True

*XpandFillForm
 Name = Bekiaris
*SaveAndClose
*SaveAndClose
*XpandProcessRecord
 ASPx Search Lookup Property Editor Lookup = Bekiaris

*Mouse LeftButtonClick
 MoveMouseTo = [ClearAction]
 ToggleNavigation = True
*SaveAndClose
*CheckValidationResult
 Message = Problems were detected during data validation*



