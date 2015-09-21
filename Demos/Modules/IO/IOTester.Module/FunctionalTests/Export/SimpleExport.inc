
*Action New(Export Object)
*XpandFillForm
 Do Not Serialize = No-Serialize
 Key = Key
*SaveAndClose
*Navigate Export Object
*SelectRecords
 Columns = Do Not Serialize
 Row = No-Serialize

;-------------------------EXPORT---------------------------------------------------
*Action IO(Export)
*XpandFillForm
 Name = Export Object Serialization Configuration
*Action Serialization Configurations.New
*XpandFillForm
 Type To Serialize = Export Object
*OptionalAction Generate Graph
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
*SaveAndClose
*Action OK

*SaveFileDialog exportobjects.xml
*Sleep 1000

;-----------------IMPORT------------------------------------------------
*Navigate Export Object
*Action Delete
*HandleDialog
 Respond = Yes
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog exportobjects.xml
*Action OK
*Action Refresh

;the delete object is retrieved because of the deffered deletion
*XpandProcessRecord
 Do Not Serialize = No-Serialize
 Key = Key

;Change the key and import again and a new object will be created
*XpandFillForm
 Key = ''
*SaveAndClose
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog exportobjects.xml
*Action OK
*Action Refresh
*XpandProcessRecord
 Do Not Serialize = ''
 Key = Key
*SaveAndClose
