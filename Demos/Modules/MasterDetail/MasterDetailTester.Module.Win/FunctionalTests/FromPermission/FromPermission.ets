#DropDB MasterDetailTesterEasyTest
#Application MasterDetailTester.Win
*UseModel ..\CommonModel
*ScreenCapture
*LogOn
#IncludeFile ..\CreateMasterDetailObjects.inc 
*CreatePermission Project;Contributor

*Action Navigation(Default.Role)
*ProcessRecord 
 Name = User
*Action Permissions
*Action Permissions.New(Logic.MasterDetail)
*FillForm
 Object Type Data = Project
 ID = Project-Contributors
*Action MasterDetailData
*XpandFillForm
 Collection Member = Contributors
 Child List View = Contributor_ListView
*Action Save and Close
*Action Save and Close

*Action Log Off
*LogOn User
*Navigate Project

#IncludeFile ..\Checks.inc 

