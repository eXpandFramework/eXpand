;#IncludeFile .\Init.inc
;#IncludeFile .\Edit.inc

*Action Navigation(LayoutViewListEditor.Edit)
*CheckTable
 RowCount != 0

*ExecuteTableAction
 SetTableFilter = [User] = 'nonexistent'
*CheckTable
 RowCount = 0

*ExecuteTableAction
 SetTableFilter = User.UserName = 'user'
*CheckTable
 RowCount = 1