;#IncludeFile .\Init.inc

*Action Full Text Contains(XpandGridListEditor)

*Action Show Clause Menu
*Sleep 2000
*LClick [ClauseMenu2]
*LClick [ClauseMenu2]
*XpandCompareScreenshot XpandGridListEditor.png
 WindowTitle = Full Text Contains Object
 ToggleNavigation = false

*KillWindow 
 WindowCaption = Filter Editor
*LogOff
*Sleep 1000
*LogOn
*Action Navigation(Default.Full Text Contains Object)

*Action Full Text Contains(AdvBandedListEditor)
*Action Show Clause Menu
*LClick [ClauseMenu]
*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object
 
*Action Cancel
*Action Cancel
