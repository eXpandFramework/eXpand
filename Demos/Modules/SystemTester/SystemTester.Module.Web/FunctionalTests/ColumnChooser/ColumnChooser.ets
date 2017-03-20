#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture

;--------------setup--------------
*Navigate Column Chooser Object
*DeleteAllObjects
*Action New
*ExecuteEditorAction Column Chooser Reference(New)
*ExecuteEditorAction Column Chooser Reference Reference(New)
*FillForm
 Reference Reference Name = Reference Reference Name
*Action OK
*FillForm
 Reference Name = Reference Name
*Action OK

*ExecuteEditorAction Column Chooser Reference 2(New)
*FillForm
 Reference Name = Reference2
*Action OK

*FillForm
 Name = Name
*SaveAndClose

;---------Hide name column-------------
*RClick [GridHeader]
 Id = GridHeader
*Sleep 1000
*LClick [ColumnChooser]
 Id = ColumnChooser
*Sleep 3000
*LClick [NodeName]
 Id = NodeName
*Sleep 1000
*Action OK
!XpandProcessRecord
 Name = Name
;---------add subreference column-------------
*RClick [GridHeader]
 id = GridHeader
*Sleep 1000
*LClick [ColumnChooser2]
 Id = ColumnChooser2
*Sleep 3000
*LClick [NodeReferenceExpand]
 Id = NodeReferenceExpand
*Sleep 2000
*LClick [NodeSubReference]
 Id = NodeSubReference
*Sleep 1000
*Action OK
*XpandProcessRecord
 Column Chooser Reference Reference = Reference Reference Name
*Navigate Column Chooser Object
;---------add subsubreference column-------------
*RClick [GridHeader]
 Id = GridHeader
*Sleep 1000
*LClick [ColumnChooser2]
 Id = ColumnChooser2
*Sleep 3000
*LClick [NodeReferenceExpand]
 Id = NodeReferenceExpand
*Sleep 2000
*LClick [NodeSubReferenceSub]
 Id = NodeSubReferenceSub
*Sleep 1000
*Action OK
*XpandProcessRecord
 Reference Reference Name = Reference Reference Name
*Navigate Column Chooser Object
;---------add same name members-------------
*RClick [GridHeader]
 Id = GridHeader
*Sleep 1000
*LClick [ColumnChooser2]
 Id = ColumnChooser2
*Sleep 3000
*LClick [Reference2Expand]
*Sleep 1000
*LClick [Reference2NodeName]
*LClick [ColumnChooserExpand]
*Sleep 100
*LClick [ColumnChooserReferenceName]
*Action OK
*Action Parameter (CheckSameCaptionColumns)
*CheckActionValue Parameter(ColumnChooserReference.ReferenceName,ColumnChooserReference2.ReferenceName)
 

 