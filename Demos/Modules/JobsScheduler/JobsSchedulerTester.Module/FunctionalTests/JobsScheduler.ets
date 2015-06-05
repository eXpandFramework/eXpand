#DropDB JobsSchedulerTesterEasyTest
#Application JobsSchedulerTester.Win
;#Application JobsSchedulerTester.Web
*ScreenCapture
*LogOn

;Create Job related objects
*Action Navigation(JobScheduler.JobDetail)
*Action New(JobDetail)
*XpandFillForm
 Name = Product Job Detaisl
*ExecuteEditorAction Job
*Action New
*XpandFillForm
 Job Type = ProductJob
*SaveAndClose
*ExecuteEditorAction Job Detail Data Map
*Action New
*XpandFillForm
 Parts = 5
 Name = product detail map
*SaveAndClose
*SaveAndClose

*Action Navigation(JobScheduler.SimpleTrigger)
*Action New(SimpleTrigger)
*XpandFillForm
 Misfire Instruction = Fire now
*Action Job Details
*Action Job Details.Link
*XpandProcessRecord 
 Name = Product Job Detaisl
*SaveAndClose

*Sleep 1000
*Action Navigation(Default.Product)
*CheckTable
 Columns = Parts
 Row[0] = 5
