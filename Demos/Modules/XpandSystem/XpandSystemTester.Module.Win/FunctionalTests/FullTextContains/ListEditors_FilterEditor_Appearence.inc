;#IncludeFile .\Init.inc

#IncludeFile .\CriteriaPropertyEditorEx_FilterEditor_Appearence .inc

*Action Full Text Contains(XpandGridListEditor)
*Action Show Clause Menu

*Sleep 1000
*XpandCompareScreenshot XpandGridListEditor.png
 WindowTitle = Full Text Contains Object
 ToggleNavigation = false

*KillWindow Filter Editor
*Action Log Off
#IncludeFile ..\Login.inc

*Action Navigation(Default.Full Text Contains Object)
*Action Full Text Contains(AdvBandedListEditor)
*Action Show Clause Menu
*Sleep 1000

*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object


*Action Cancel
*Action Cancel
