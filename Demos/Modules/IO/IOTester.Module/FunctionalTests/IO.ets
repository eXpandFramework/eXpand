#DropDB IOTesterEasyTest
#Application IOTester.Win
#Application IOTester.Web

*ScreenCapture
*LogOn

*Navigate IOObject
*Action New(IOObject)
*XpandFillForm
 Do Not Serialize = No-Serialize
 Key = Key
*SaveAndClose
*Navigate IOObject
*SelectRecords
 Columns = Do Not Serialize
 Row = No-Serialize

;-------------------------EXPORT---------------------------------------------------
*Action IO(Export)
*XpandFillForm
 Name = IOObject Serialization Configuration
*Action Serialization Configurations.New
*XpandFillForm
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
*Action OK

*Sleep 1000
*SaveFileDialog exportobjects.xml
*Sleep 1000
*OptionalAction Cancel

;-----------------IMPORT------------------------------------------------
*Navigate IOObject
*SelectRecords
 Columns = Key
 Row = Key
*Action Delete
*Sleep 1000
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
