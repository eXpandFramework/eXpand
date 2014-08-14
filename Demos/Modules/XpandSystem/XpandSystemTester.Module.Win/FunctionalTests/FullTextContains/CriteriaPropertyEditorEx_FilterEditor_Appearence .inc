;#IncludeFile .\Init.inc
*Action Full Text Contains(CriteriaPropertyEditorEx)
*SetActiveWindowSize 640x480
*FillForm
 Data Type = Full Text Contains Object
*Action Show Clause Menu(CriteriaPropertyEditorEx)
*Sleep 1000

*XpandCompareScreenshot CriteriaPropertyEditorEx.png

*Action Cancel
*HandleDialog
 Respond = No