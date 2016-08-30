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
*FillForm
 Name = Name
*SaveAndClose

;---------Hide name column-------------
*RClick [GridHeader]
*Sleep 1000
*LClick [ColumnChooser]
*Sleep 3000
*LClick [NodeName]
*Sleep 1000
*Action OK
!XpandProcessRecord
 Name = Name
;---------add subreference column-------------
*RClick [GridHeader]
*Sleep 1000
*LClick [ColumnChooser2]
*Sleep 3000
*LClick [NodeReferenceExpand]
*Sleep 2000
*LClick [NodeSubReference]
*Sleep 1000
*Action OK
*XpandProcessRecord
 Column Chooser Reference Reference = Reference Reference Name
*Navigate Column Chooser Object
;---------add subsubreference column-------------
*RClick [GridHeader]
*Sleep 1000
*LClick [ColumnChooser2]
*Sleep 3000
*LClick [NodeReferenceExpand]
*Sleep 2000
*LClick [NodeSubReferenceExpand]
*Sleep 1000
*LClick [NodeSubReferenceSub]
*Sleep 1000
*Action OK
*XpandProcessRecord
 Reference Reference Name = Reference Reference Name
*Navigate Column Chooser Object