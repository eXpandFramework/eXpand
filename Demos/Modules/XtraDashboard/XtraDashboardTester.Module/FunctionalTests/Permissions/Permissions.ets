#DropDB XtraDashboardTesterEasyTest
#Application XtraDashboardTester.Win
#Application XtraDashboardTester.Web

*LogOn User
!ActionVisible Navigation(Dashboards.Filtered from model)
*LogOff

*LogOn Admin

*Action Navigation(Default.Role)
*XpandProcessRecord 
 Name = User
*Action Dashboard Definitions
*XpandFillForm
 Dashboard Operation = Read only access
*Action Dashboard Definitions.Link
*SelectRecords
 Columns = Name 
 Row[0] = Filtered from model
*SaveAndClose
*SaveAndClose
*LogOff

*LogOn User
*ActionVisible Navigation(Dashboards.Filtered from model)