#IncludeFile .\Init.inc

*Action Navigation(PreventDataLoading.FilterNotEmpty)
*CheckTable 
 RowCount = 2

*Action Prevent Data Loading(ActiveFilterEnabled)
*CheckTable 
 RowCount = 0
*Action Prevent Data Loading(ActiveFilterEnabled)
*CheckTable 
 RowCount = 2
*Action Prevent Data Loading(ActiveFilter)
*CheckTable 
 RowCount = 1

*Action Navigation(PreventDataLoading.Default)
*Action Prevent Data Loading(ActiveFilter)
*CheckTable 
 RowCount = 1
*Action Prevent Data Loading(ActiveFilterEnabled)
*CheckTable 
 RowCount = 3