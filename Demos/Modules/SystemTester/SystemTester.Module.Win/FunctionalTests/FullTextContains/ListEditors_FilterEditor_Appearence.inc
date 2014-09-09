;#IncludeFile .\Init.inc

#IncludeFile .\CriteriaPropertyEditorEx_FilterEditor_Appearence .inc

*Action Full Text Contains(XpandGridListEditor)
*Action Show Clause Menu
*XpandCompareScreenshot XpandGridListEditor.png
 WindowTitle = Full Text Contains Object
 ToggleNavigation = false

*KillWindow Filter Editor
*Action Log Off
*Action Log On
*Action Navigation(Default.Full Text Contains Object)

*Action Full Text Contains(AdvBandedListEditor)
*Action Show Clause Menu
*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object
 
*Action Cancel
*Action Cancel
