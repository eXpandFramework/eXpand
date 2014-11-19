*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action New(Persistent Assembly Info)
*FillForm
 Name = MapLegacyDBAssembly
 Is Legacy = True
*Action Tools(Map Datastore)
*FillForm
 Connection String = XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=WCDBMapperTester
 Navigation Path = WCDBMapper
*Action OK
*Action Save and Close