;#IncludeFile .\Init.inc

#IncludeFile .\CriteriaPropertyEditorEx_FilterEditor_Appearence .inc

*Action Full Text Contains(XpandGridListEditor)
*SetActiveWindowSize 640x480
*Action Show Clause Menu

*Sleep 1000
*XpandCompareScreenshot XpandGridListEditor.png
 WindowTitle = Full Text Contains Object

*KillWindow Filter Editor
*Action Log Off
#IncludeFile ..\Login.inc

*Action Navigation(Default.Full Text Contains Object)
*Action Full Text Contains(AdvBandedListEditor)
*SetActiveWindowSize 640x480
*Action Show Clause Menu
*Sleep 1000

*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object


*Action Cancel
*Action Cancel
