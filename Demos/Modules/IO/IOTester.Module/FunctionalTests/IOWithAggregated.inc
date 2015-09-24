
;---------------- EXPORT OBJECTS WITH CHILD OBJECTS -----------------

;Set object as key on serialization configuration
*Action Navigation(ImportExport.Serialization Configuration Group)
*Action New(Serialization Configuration Group)
*FillForm
 Name = Export Object With Children Serialization Configuration
*Action Serialization Configurations.New
*FillForm
 Type To Serialize = IOObject
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
 Type To Serialize = IOAggregated Object
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
*Navigate IOObject
*Action New(IOObject)
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
*Action New(IOObject)
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
*Navigate IOObject
*SelectRecords IOObject
 Columns = Key
 Row = Object1
 Row = Object2
*Action IO(Export)
*SelectRecords Serialization Configuration Group
 Columns = Name
 Row = Export Object With Children Serialization Configuration
*Action OK
*Sleep 1000
*SaveFileDialog objectswithChildren.xml
*Sleep 1000
*OptionalAction Cancel

*Action Delete
*HandleDialog
 Respond = Yes
 
;Import and all four children will be imported in respective parents.
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog objectswithChildren.xml
*Action OK
*Action Refresh

;Confirm number of child objects
*Navigate IOAggregated Object
*CheckTable
 RowCount = 4

;Change a parent key and a new object will be created with children
*Navigate IOObject
*XpandProcessRecord
 Key = Object2
*XpandFillForm
 Key = Object3
*SaveAndClose
*Action Refresh

*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog objectswithChildren.xml
*Action OK
*Action Refresh

;Confirm number of child objects
*Navigate IOAggregated Object
*CheckTable
 RowCount = 6