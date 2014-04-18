;#IncludeFile .\Init.inc
*UseModel ImmediatePostData
 IsExternalModel = True
*Navigate GridView.ImmediateRefresh
*Action New(Grid View Object)
*SaveAndClose
*Navigate GridView.ImmediateRefresh
*FocusWindow
*Mouse LeftButtonClick
 MoveMouseTo = 450x350
*SendKeys 2
*CheckTable
 Columns = Number Alias
 Row[0] = 2
