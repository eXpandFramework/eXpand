;#IncludeFile .\Init.inc

*Action Navigation(LayoutViewListEditor.Edit)
*Action New(Layout View List Editor Object)
*XpandFillForm
 User = Admin
*Action Save and Close

*ExecuteTableAction
 User = Admin
 InlineEdit = ''
*FillRecord
 Columns = User
 Values = user
*Action Save and Close

*Action Navigation(LayoutViewListEditor.Edit)
*XpandProcessRecord
 User = user