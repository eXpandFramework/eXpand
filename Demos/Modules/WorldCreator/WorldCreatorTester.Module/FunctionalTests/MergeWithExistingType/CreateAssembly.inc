*Action Navigation(Merge.ExistingTypeToMergeWith)
*Action New(Existing Type To Merge With)
*FillForm
 First Name = Tolis
 Last Name = Bekiaris
*SaveAndClose

*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action New(Persistent Assembly Info)
*FillForm
 Name = ExtendExistentTypeAssembly
*Action Persistent Class Infos
*Action Persistent Class Infos.New
*FillForm
 Base Type = Existing Type To Merge With
 Merged Object Type = Existing Type To Merge With
*FillForm
 Name = ExtendedType
 

*Action Type Attributes
*Action Type Attributes.New(MapInheritance)
*SaveAndClose
*Action Type Attributes.New(Xpand Navigation)
*FillForm
 View Id = ExtendedType_ListView
 Path = Merge/ExtendedType
*SaveAndClose

*Action Own Members
*Action Own Members.New(Persistent Core Type Member Info)
*FillForm
 Data Type = String
 Name = TestProp
*SaveAndClose
*SaveAndClose
*Action Save

*ReopenApplication
*Action Log On