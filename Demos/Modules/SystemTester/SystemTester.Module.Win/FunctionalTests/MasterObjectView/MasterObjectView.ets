#IncludeFile .\Init.inc

*Action Navigation(Default.Actions Object)
*Action Navigation(MasterObjectView.Master)
*Action New(Master Object View Master)
*FillForm
 Name = Master
*Action Save and Close
*Action Navigation(MasterObjectView.Nested)
*Action New(Master Object View Nested)
*FillForm
 Master Object View Master = Master
 Name = test1
*Action Save and Close
*Action New(Master Object View Nested)
*FillForm
 Name = test2
*Action Save and Close
*Action Navigation(MasterObjectView.Master)
*ProcessRecord
 Name = Master
*CheckTable Nesteds
 Columns = Name
 RowCount = 1
 Row[0] = test1
 