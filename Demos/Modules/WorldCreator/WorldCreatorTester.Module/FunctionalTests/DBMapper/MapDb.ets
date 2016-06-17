#IncludeFile .\Init.inc

;----------------------Create Mapped Assembly----------------
*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action New(Persistent Assembly Info)
*XpandFillForm
 Name = MapLegacyDBAssembly
*Action Tools(Map Datastore)
*XpandFillForm
 Connection String = XpoProvider=MSSqlServer;data source=(localdb)\mssqllocaldb;integrated security=SSPI;initial catalog=WCDBMapperTester
 Navigation Path = WCDBMapper
*Action Populate
*ExecuteTableAction DBObjects
 SelectAll = True
*SaveAndClose
*SaveAndClose

;----------------Check Mapped Assembly-------------------------
*ReopenApplication
*LogOn
*Action Navigation(WCDBMapper.WCTestTable)
*Action New(WCTest Table) 
*XpandFillForm
 Name = Tolis
*Action Save and Close
*Action Navigation(WCDBMapper.WCTestTable)
*CheckTable
 Columns = Name
 Row[0] = Tolis*

*Sql 'Integrated Security=SSPI;Data Source=(localdb)\MSSQLLocalDB'
 ScalarCommand = USE WCDBMapperTester SELECT COUNT(*) from information_schema.tables WHERE table_type = 'base table' 
 Result = 1




