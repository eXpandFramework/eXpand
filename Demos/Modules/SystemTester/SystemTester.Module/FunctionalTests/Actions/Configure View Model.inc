;#IncludeFile .\Init.inc

*Action New(Actions Object)
*FillForm
 Property Name = visible
*Action Save and Close
*Action New(Actions Object)
*FillForm
 Property Name = notvisible
*Action Save and Close
*Action Modify Model(Change View Model)
*FillForm
 Criteria = StartsWith([PropertyName], 'v')
*Action OK

*CheckTable
 RowCount = 1
*Action Modify Model(Change View Model)
*FillForm
 Criteria = ''
*Action OK
*CheckTable
 RowCount = 2

*SelectRecords Actions Object
 Columns = Property Name
 Row = notvisible
*Action Delete
*HandleDialog
 Respond = Yes
*SelectRecords Actions Object
 Columns = Property Name
 Row = visible
*Action Delete
*HandleDialog
 Respond = Yes