;#IncludeFile .\Init.inc
*Action Full Text Contains(CriteriaPropertyEditorEx)
*FillForm
 Data Type = Full Text Contains Object
*Action Show Clause Menu(CriteriaPropertyEditorEx)
*LClick [ClauseMenu]

*XpandCompareScreenshot CriteriaPropertyEditorEx.png
 ToggleNavigation = False

*Action Cancel
*HandleDialog
 Respond = No