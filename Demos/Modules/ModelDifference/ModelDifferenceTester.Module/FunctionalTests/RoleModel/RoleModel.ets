#DropDB ModelDifferenceTesterEasyTest
#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*UseModel
*LogOn
*ScreenCapture
*Navigate Application Difference
*SelectRecords Application Difference
 Columns = Difference Type
 Row = Model

*Action Clone...(Role Difference)
*Action General
*XpandFillForm
 Name = User Role
 Disabled = False
*Action Roles
*Action Roles.Link
*ClearSelection
*SelectRecords 
 Columns = Name
 Row = User
*Action OK
*Action Xml Content
*XpandFillForm
 Xml Content = <Application><Views><ListView Id="RoleModelObject_ListView"><Columns><ColumnInfo Id="Name" Caption="Name_Changed" /></Columns></ListView></Views></Application>
*Action Save and Close


*Action Log Off
*LogOn User
*Navigate Default.Role Model Object
*CheckTable
 Columns = Name_Changed
*Action Log Off
*LogOn Admin
*Navigate Default.Role Model Object
*CheckTable
 Columns = Name

  