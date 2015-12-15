#DropDB WorkflowTesterEasyTest
#Application WorkflowTester.Win
*ScreenCapture

;--------------Create WC Assembly
*Action Navigation(WorldCreator.Persistent Assembly Info)
*Action New
*FillForm
 Name = TestAssembly
*Action Persistent Class Infos
*Action Persistent Class Infos.New
*FillForm
 Name = TestClass
*Action Type Attributes
*Action Type Attributes.New(DefaultClassOptions)
*Action Save and Close
*Action Own Members
*Action Own Members.New(Core)
*FillForm
 Data Type = String
 Name = TestProperty
*SaveAndClose
*SaveAndClose
*SaveAndClose
*ReopenApplication

*Action Navigation(Workflow.Workflow Definition)
*Action New(Workflow Definition)
*Action Properties
*FillForm
 Name = test
 Target Object Type = Test Class
*Action Assign Xaml (WorldCreatorWorkflow)
*SaveAndClose
*XpandProcessRecord
 Target Object Type = Test Class
*Action Designer
*Action Activate
*Action Debug
*Sleep 20000
*SaveAndClose
*Sleep 1000
*ReopenApplication
*Navigate Test Class
*CheckTable 
 RowCount = 1
