#IncludeFile .\Init.inc 

*Navigate Type As Serialization Key
*Action New(Type As Serialization Key)
*XpandFillForm
 Type Key = Type As Serialization Key
*SaveAndClose
*Navigate Type As Serialization Key
*SelectRecords
 Columns = Type Key
 Row = Type As Serialization Key

;-------------------------EXPORT---------------------------------------------------
*Action Navigation(ImportExport.Serialization Configuration Group)
*Action New
*XpandFillForm
 Name = TypeAsSerializationKey
*Action Serialization Configurations.New
*XpandFillForm
 Type To Serialize = Type As Serialization Key
*OptionalAction Generate Graph
*XpandFillRecord Serialization Graph
 Columns = Serialization Strategy, Key
 Values = Do not serialize, False
 Name = oid
*XpandFillRecord Serialization Graph
 Columns = Key
 Values = True
 Name = TypeKey
*SaveAndClose
*SaveAndClose

*Navigate Type As Serialization Key
*SelectRecords
 Columns = Type Key
 Row = Type As Serialization Key
*Action IO(Export)
*SelectRecords
 Columns = Name
 Row = TypeAsSerializationKey
*Action OK
*Sleep 1000
*SaveFileDialog TypeAsSerializationKey.xml
*Sleep 1000
*OptionalAction Cancel

;-----------------IMPORT------------------------------------------------
*Navigate Type As Serialization Key
*SelectRecords
 Columns = Type Key
 Row = Type As Serialization Key
*Action Delete
*Sleep 1000
*HandleDialog
 Respond = Yes
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog TypeAsSerializationKey.xml
*Action OK
*Action Refresh

;--------------the delete object is retrieved because of the deffered deletion-------------
*XpandProcessRecord
 Type Key = Type As Serialization Key

;-------------Change the key and import again and a new object will be created----------------
*XpandFillForm
 Type Key = User
*SaveAndClose
*Action IO(Import)
*ExecuteEditorAction File Data
*OpenFileDialog TypeAsSerializationKey.xml
*Action OK
*Action Refresh
*XpandProcessRecord
 Type Key = Type As Serialization Key
