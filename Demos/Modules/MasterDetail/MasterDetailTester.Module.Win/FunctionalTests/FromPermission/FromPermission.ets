#DropDB MasterDetailTesterEasyTest
#Application MasterDetailTester.Win
*UseModel ..\CommonModel
*ScreenCapture
*Action Log On
#IncludeFile ..\CreateMasterDetailObjects.inc 
*CreatePermission Project;Contributor

*Action Navigation(Default.Role)
*ProcessRecord 
 Name = User
*Action Permissions
*Action Permissions.New
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
*FillForm
 User Name = User
*Action Log On
*Navigate Project

#IncludeFile ..\Checks.inc 

