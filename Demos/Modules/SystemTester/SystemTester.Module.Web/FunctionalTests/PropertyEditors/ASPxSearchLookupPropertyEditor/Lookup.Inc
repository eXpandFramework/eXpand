;#IncludeFile .\Init.inc


*Navigate ASPx Search Lookup Property Editor Object
*ToggleNavigation = True
*Action New
*LClick [NewAction]


*XpandFillForm
 Name = Tolis
*SaveAndClose
*ToggleNavigation = False
*Navigate ASPx Search Lookup Property Editor Object
*Action New
*SendKeys Tol
*Sleep 3000
*SendKeys 
 Keys = Return
*SaveAndClose
*XpandProcessRecord
 ASPx Search Lookup Property Editor Lookup = Tolis

