#DropDB WorkflowTesterEasyTest
#Application WorkflowTester.Win
*ScreenCapture


;-----Create Workflow definition-------------------------
*Action Navigation(Workflow.Scheduled Workflow)
*Action New
*FillForm
 Name = test
 Is Active = True
*Action Assign Xaml
*Action Launch Schedule Items.New
*FillForm
 Start Mode = One time
*Action Save and Close
*Action Save and Close

;----missed workflow executes----------
*ReopenApplication

;-----------Check workflow output
*Action Navigation(Default.Demo Task)
*CheckTable 
 RowCount = 1

*Action Navigation(Workflow.Scheduled Workflow)
*ProcessRecord
 Name = test
*CheckTable Launch History Items
 RowCount = 1