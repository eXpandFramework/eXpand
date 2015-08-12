#DropDB ModelDifferenceTesterEasyTest
#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*LogOn
*ScreenCapture
*Navigate Application Difference

*XpandProcessRecord
 Name = *Admin*
 Action = Edit

*Action Xml Content
*FillForm
 Xml Content = <Application><Views><ListView Id="UserModelObject_ListView"><Columns><ColumnInfo Id="Name" Caption="Name_Changed" /></Columns></ListView></Views></Application>
*SaveAndClose
*Navigate Default.User Model Object
*CheckTable
 Columns = Name_Changed
  