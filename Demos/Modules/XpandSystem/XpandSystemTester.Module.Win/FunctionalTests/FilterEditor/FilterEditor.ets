#IncludeFile .\Init.inc

#IncludeFile .\FilterEditorTop.inc

;create objects
*Action New(Filter Editor Object)
*ExecuteEditorAction Filter Editor Object Ref
*Action New
*FillForm
 Name = ref
*Action Save and Close
*FillForm
 Name = editor
*Action Save and Close
*CheckTable
 RowCount = 1
 Columns = Name, Filter Editor Object Ref
 Row[0] = editor, ref

;create one more object set
*Action New(Filter Editor Object)
*ExecuteEditorAction Filter Editor Object Ref
*Action New
*FillForm
 Name = ref1
*Action Save and Close
*FillForm
 Name = editor1
*Action Save and Close
*CheckTable
 RowCount = 2
 Columns = Name, Filter Editor Object Ref
 Row[1] = editor1, ref1

*Action Navigation(FilterEditor.Criteria)
*CheckTable
 RowCount = 1 