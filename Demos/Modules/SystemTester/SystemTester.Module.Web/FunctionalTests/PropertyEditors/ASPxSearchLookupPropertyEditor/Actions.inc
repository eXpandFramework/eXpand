;#IncludeFile .\Init.inc
*Navigate ASPx Search Lookup Property Editor Object
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



