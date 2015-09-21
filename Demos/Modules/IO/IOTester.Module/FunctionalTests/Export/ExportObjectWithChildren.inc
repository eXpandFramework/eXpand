
;---------------- EXPORT OBJECTS WITH CHILD OBJECTS -----------------

;Set object as key on serialization configuration
*Action Navigation(ImportExport.Serialization Configuration Group)
*Action New(Serialization Configuration Group)
*FillForm
 Name = Export Object With Children Serialization Configuration
*Action Serialization Configurations.New
*FillForm
 Type To Serialize = Export Object
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy
 Values = Do not serialize
 SelectColumn = Name
 SelectRow = DoNotSerialize
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy, Key
 Values = Do not serialize, False
 SelectColumn = Name
 SelectRow = oid
*XpandFillRecord Serialization Graph
 Columns = Key
 Values = True
 SelectColumn = Name
 SelectRow = Key
*Action Save and Close
*Action Refresh
*XpandProcessRecord Serialization Configurations
 Type To Serialize = Export Aggregated Object
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy, Key
 Values = Do not serialize, False
 SelectColumn = Name
 SelectRow = oid
*XpandFillRecord Serialization Graph
 Columns = Key
 Values = True
 SelectColumn = Name
 SelectRow = NestedKey
;Check if SerializationKey attribute works
*XpandProcessRecord Serialization Graph
 Name = ParentObject
 Key = True
*Action Save and Close
*Action Save and Close
*Action Save and Close

;Create objects with child
*Action Navigation(Default.Export Object)
*Action New(Export Object)
*FillForm
 Key = Object1
*Action Aggregated Objects.New
*FillForm
 Nested Key = Child1
*Action Save and Close
*Action Aggregated Objects.New
*FillForm
 Nested Key = Child2
*Action Save and Close
*Action Save and Close
*Action New(Export Object)
*FillForm
 Key = Object2
*Action Aggregated Objects.New
*FillForm
 Nested Key = Child1
*Action Save and Close
*Action Aggregated Objects.New
*FillForm
 Nested Key = Child2
*Action Save and Close
*Action Save and Close

;ExportRecords then delete
*Action Navigation(Default.Export Object)
*SelectRecords Export Object
 Columns = Key
 Row = Object1
 Row = Object2
*Action IO(Export)
*SelectRecords Serialization Configuration Group
 Columns = Name
 Row = Export Object With Children Serialization Configuration
*Action OK
*SaveFileDialog exportobjectswithChildren.xml
*Sleep 1000
*Action Delete
*HandleDialog
 Respond = Yes
 
;Import and all four children will be imported in respective parents.
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog exportobjectswithChildren.xml
*Action OK
*Action Refresh

;Confirm number of child objects
*Action Navigation(Default.Export Aggregated Object)
*CheckTable
 RowCount = 4

;Change a parent key and a new object will be created with children
*Action Navigation(Default.Export Object)
*XpandProcessRecord
 Key = Object2
*XpandFillForm
 Key = Object3
*SaveAndClose
*Action Refresh

*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog exportobjectswithChildren.xml
*Action OK
*Action Refresh

;Confirm number of child objects
*Action Navigation(Default.Export Aggregated Object)
*CheckTable
 RowCount = 6