#IncludeFile .\Init.inc

*Action Navigation(NestedListViewNonPersistentObjectSpace.Master)
*Action New(Nested List View Non Persistent Object Space Master)
*FillForm
 Master Name = test
*Action Save
*Action Refresh
*CheckTable Nesteds
 Columns = Nested Name
 RowCount = 1 
 Row[0] = test
*Action Save and Close
*Action Navigation(NestedListViewNonPersistentObjectSpace.Master)
*ProcessRecord 
 Master Name = test
*CheckTable Nesteds
 Columns = Nested Name
 RowCount = 1 
 Row[0] = test
 