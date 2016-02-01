#IncludeFile .\Init.inc

*Action Navigation(Default.Filter By Column Object)
*Action New(Filter By Column Object)
*FillForm
 Name = base object
*SaveAndClose
*Action New(Filter By Column Object Derived)
*FillForm
 Derived Property = propertyValue
 Name = DerivedObject
*SaveAndClose
*ClearSelection
*SelectRecords Filter By Column Object
 Columns = Name
 Row = DerivedObject
*SendKeys
 Keys = Right
*Action Cell Filter
*CheckTable
 RowCount = 1
*CheckTableSelection
 Columns = Name
 Row = DerivedObject