#DropDB WorkflowTesterEasyTest
#Application WorkflowTester.Win
;#Application WorkflowTester.Web
*ScreenCapture


;-----Create Workflow definition-------------------------
*Action Navigation(Workflow.Object Changed Workflow)
*Action New(Object Changed Workflow)
*Action Properties
*FillForm
 Name = test
 Target Object Type = Object Changed Workflow Object
 Execution Domain = Client
 Property Name = PropertyName
 Is Active = True
*Action Assign Xaml

*Action Save and Close

;-----Change  the object proeprty that triggers the workflow
*Action Navigation(Default.Object Changed Workflow Object)
*Action New(Object Changed Workflow Object)
*FillForm
 Property Name = changevalue
*Action Save and Close


;-----------Check workflow output
*Action Navigation(Default.Demo Task)
*CheckTable 
 RowCount = 1