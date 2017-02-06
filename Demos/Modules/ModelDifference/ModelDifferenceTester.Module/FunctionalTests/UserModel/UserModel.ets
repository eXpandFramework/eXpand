#DropDB ModelDifferenceTesterEasyTest
#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*LogOn
*ScreenCapture

;-------change xml of user model---------------------
*Navigate Application Settings
*XpandProcessRecord
 Name = *Admin*
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = <Application><Views><ListView Id="UserModelObject_ListView"><Columns><ColumnInfo Id="Name" Caption="Name_Changed" /></Columns></ListView></Views></Application>
*SaveAndClose

;-------update xml of user model---------------------
*Navigate Application Settings
*XpandProcessRecord
 Name = *Admin*
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = <Application><Views><ListView Id="UserModelObject_ListView"><Columns><ColumnInfo Id="Name" Caption="Name_Changed_Updated" /></Columns></ListView></Views></Application>
*SaveAndClose

;---------check that model is applied----------------
*Navigate Default.User Model Object
*CheckTable
 Columns = Name_Changed_Updated

*LogOff

;------check that model is applied only to current user-------------
*LogOn User2
*Navigate Default.User Model Object
*CheckTable
 Columns = Name

  