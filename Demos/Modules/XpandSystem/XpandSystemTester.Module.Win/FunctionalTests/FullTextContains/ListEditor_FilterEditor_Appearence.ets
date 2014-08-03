#IncludeFile Init.inc

*Action Model Value(Views/FullTextContainsObject_LookupListView;EditorType;Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.XpandGridListEditor)
#IncludeFile ListEditor_FilterEditor_Appearence.inc
*Sleep 1000
*XpandCompareScreenshot XpandGridListEditor.png
 WindowTitle = Full Text Contains Object

*KillWindow Filter Editor
*Action Log Off
#IncludeFile ..\Login.inc

*Action Model Value(Views/FullTextContainsObject_LookupListView;EditorType;Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.AdvBandedListEditor)
#IncludeFile ListEditor_FilterEditor_Appearence.inc
*Sleep 1000
*XpandCompareScreenshot AdvBandedListEditor.png
 WindowTitle = Full Text Contains Object


