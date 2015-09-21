;---------------- IOObjectS WITH CHILD OBJECTS -----------------

;--Set object as key on serialization configuration
*Action Navigation(ImportExport.Serialization Configuration Group)
*Action New(Serialization Configuration Group)
*FillForm
 Name = IOObject With Children Serialization Configuration
*Action Serialization Configurations.New
*FillForm
 Type To Serialize = IOObject
*OptionalAction Generate Graph
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy
 Values = Do not serialize
 Name = DoNotSerialize
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy, Key
 Values = Do not serialize, False
 Name = oid
*XpandFillRecord Serialization Graph
 Columns = Key
 Values = True
 Name = Key
*SaveAndClose
*Action Refresh
*XpandProcessRecord Serialization Configurations
 Type To Serialize = Export Aggregated Object
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy, Key
 Values = Do not serialize, False
 Name = oid
*XpandFillRecord Serialization Graph
 Columns = Key
 Values = True
 Name = NestedKey
;--Check if SerializationKey attribute works
*XpandProcessRecord Serialization Graph
 Name = ParentIOObject
 Key = True
*SaveAndClose
*SaveAndClose
;*SaveAndClose

;Create objects with child
*Action Navigation(Default.IOObject)
*Action New(IOObject)
*XpandFillForm
 Key = Object1
*Action Aggregated Objects.New
*XpandFillForm
 Nested Key = Child1
*SaveAndClose
*Action Aggregated Objects.New
*XpandFillForm
 Nested Key = Child2
*SaveAndClose
*SaveAndClose
*Action New(IOObject)
*FillForm
 Key = Object2
*Action Aggregated Objects.New
*XpandFillForm
 Nested Key = Child1
*SaveAndClose
*Action Aggregated Objects.New
*XpandFillForm
 Nested Key = Child2
*SaveAndClose
*SaveAndClose

;--ExportRecords then delete
*Action Navigation(Default.IOObject)
*SelectRecords IOObject
 Columns = Key
 Row = Object1
 Row = Object2
*Action IO(Export)
*SelectRecords Serialization Configuration Group
 Columns = Name
 Row = IOObject With Children Serialization Configuration
*Action OK
*SaveFileDialog exportobjectswithChildren.xml
*Sleep 1000
*OptionalAction Cancel
*OptionalAction Save and Close
*ExecuteTableAction
 SelectAll = True
*Action Delete
*HandleDialog
 Respond = Yes
 
;--Import and all four children will be imported in respective parents.
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog exportobjectswithChildren.xml
*Action OK
*Action Refresh

;--Confirm number of child objects
*Action Navigation(Default.Export Aggregated Object)
*CheckTable
 RowCount = 4

;--Change a parent key and a new object will be created with children
*Action Navigation(Default.IOObject)
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

;--Confirm number of child objects
*Action Navigation(Default.Export Aggregated Object)
*CheckTable
 RowCount = 6