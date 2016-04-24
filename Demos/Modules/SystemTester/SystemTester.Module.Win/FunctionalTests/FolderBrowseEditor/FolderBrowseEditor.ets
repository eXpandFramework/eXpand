#IncludeFile .\Init.inc

*Action Navigation(Default.Folder Browse Editor Object)
*Action New
*XpandExecuteEditorAction Path(Dialog)
*Sleep 1000
*SendKeys
 Keys = Return
 WindowCaption = Browse For Folder
*Sleep 1000
*XpandCheckFieldValues
 Path = C:\Users\[UserName]\Desktop
*XpandExecuteEditorAction Path(Browse)
*SaveAndClose
*XpandProcessRecord
 Path = C:\Users\[UserName]\Desktop
*Action Navigation(Default.Folder Browse Editor Object)
*Action Navigation(Default.Folder Browse Editor Object)




