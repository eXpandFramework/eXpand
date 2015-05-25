;#IncludeFile .\Init.inc
*UseModel GridViewOptions
 IsExternalModel = True
*Navigate GridView.Options
*XpandCompareScreenshot GridViewOptions.png
 MaskRectangle = 18x92;479x26

;test CheckBoxRowSelect
*Action New
*SaveAndClose
*Action New
*SaveAndClose
*Mouse LeftButtonClick
 MoveMouseTo = 261x137
*Action Delete
*HandleDialog
 Respond = Yes
*CheckTable
 RowCount = 0