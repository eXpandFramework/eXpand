*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action New(Persistent Assembly Info)
*FillForm
 Name = OneToMany
*Action Persistent Class Infos

*Action Persistent Class Infos.New
*FillForm
 Name = Project

*Action Type Attributes
*Action Type Attributes.New(DefaultClassOptions)
*SaveAndClose

*Action Own Members
*Action Own Members.New(Persistent Core Type Member Info)
*FillForm
 Data Type = String
 Name = Name
*SaveAndClose
*SaveAndClose


*Action Persistent Class Infos.New
*FillForm
 Name = Contributor
*Action Own Members
*Action Own Members.New(Persistent Core Type Member Info)
*FillForm
 Data Type = String
 Name = Name
*SaveAndClose
*SaveAndClose

*Action Save

*XpandProcessRecord Persistent Class Infos
 Name = Project
 Action = Edit
*Action Own Members
*Action Own Members.New(Persistent Collection Member Info)
*FillForm
 Collection Class Info = Contributor
 Name = Contributors

*Action Type Attributes
*Action Type Attributes.New(Association)
*FillForm
 Association Name = Project-Contributors
*SaveAndClose

*SaveAndClose
*SaveAndClose

*XpandProcessRecord Persistent Class Infos
 Name = Contributor
 Action = Edit
*Action Own Members
*Action Own Members.New(Persistent Reference Member Info)
*FillForm
 Reference Class Info = Project
 Name = Project

*Action Type Attributes
*Action Type Attributes.New(Association)
*FillForm
 Association Name = Project-Contributors
*SaveAndClose

*SaveAndClose
*SaveAndClose

*Action Save

*ReopenApplication
*LogOn