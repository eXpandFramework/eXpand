#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture

*Navigate ASPx Grid VIew Object
*DeleteAllObjects
*Action New
*FillForm
 Name = Tolis
*SaveAndClose
*Navigate ASPx Grid VIew Object
*Action New
*FillForm
 Name = Bekiaris
*SaveAndClose
*Navigate ASPx Grid VIew Object
*ExecuteTableAction
 SetTableFilter = Name=e
*CheckTable
 RowCount = 1
*XpandProcessRecord
 Name = Bekiaris
