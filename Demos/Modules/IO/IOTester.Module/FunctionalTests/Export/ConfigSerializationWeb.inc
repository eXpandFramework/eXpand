#IfDef IOTester.Web
*ClearSelection Serialization Graph
*SelectRecords Serialization Graph
 Columns = Name
 Row = DoNotSerialize
*XpandFillForm
 Serialization Graph.Serialization Strategy = Do not serialize
*SelectRecords Serialization Graph
 Columns = Name
 Row = oid
*XpandFillForm
 Serialization Graph.Serialization Strategy = Do not serialize
 Serialization Graph.Key = False
*SelectRecords Serialization Graph
 Columns = Name
 Row = Key
*XpandFillForm
 Serialization Graph.Key = True
*SaveAndClose
#EndIf