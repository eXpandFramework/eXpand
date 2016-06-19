;#IncludeFile .\Init.inc


*Navigate ASPx Search Lookup Property Editor Object
*ToggleNavigation = True
*Action New
*LClick [NewAction]
*ToggleNavigation = False

*XpandFillForm
 Name = Tolis
*SaveAndClose
*Navigate ASPx Search Lookup Property Editor Object
*Action New
*SendKeys Tol
*Sleep 3000
*SendKeys 
 Keys = Return
*SaveAndClose
*XpandProcessRecord
 ASPx Search Lookup Property Editor Lookup = Tolis

