;#IncludeFile .\Init.inc

*Action Navigation(PreventDataLoading.FiltersNotEmpty)
*CheckTable 
 RowCount = 0

*Action Filter(CommonFilter)
*CheckTable 
 RowCount = 1
*Action Filter(All)
*CheckTable 
 RowCount = 3

*Action Filter(CommonFilter)
