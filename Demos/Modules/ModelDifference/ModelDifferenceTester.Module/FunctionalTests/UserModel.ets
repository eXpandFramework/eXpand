#DropDB ModelDifferenceTesterEasyTest
#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*Action Log On
*ScreenCapture
*Navigate Application Difference

*XpandProcessRecord
 Name = AutoCreated for Admin*
 Action = Edit

*Action Xml Content
*FillForm
 Xml Content = <Application><Views><ListView Id="DifferenceTesterObject_ListView"><Columns><ColumnInfo Id="Name" Caption="Name_Changed" /></Columns></ListView></Views></Application>
*SaveAndClose
*Navigate Default.Difference Tester Object
*CheckTable
 Columns = Name_Changed
  