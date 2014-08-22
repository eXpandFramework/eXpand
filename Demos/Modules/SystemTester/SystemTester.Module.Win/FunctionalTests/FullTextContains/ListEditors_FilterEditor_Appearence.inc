;#IncludeFile .\Init.inc

#IncludeFile .\CriteriaPropertyEditorEx_FilterEditor_Appearence .inc

*Action Full Text Contains(XpandGridListEditor)
*Action Show Clause Menu

*XpandCompareScreenshot XpandGridListEditor.png
 ToggleNavigation = False
 WindowTitle = Full Text Contains Object

*KillWindow Filter Editor
*Action Log Off
*Action Log On

*Action Navigation(Default.Full Text Contains Object)
*Action Full Text Contains(AdvBandedListEditor)
*Action Show Clause Menu
*Sleep 1000

*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object
 ToggleNavigation = False

*Action Cancel
*Action Cancel
