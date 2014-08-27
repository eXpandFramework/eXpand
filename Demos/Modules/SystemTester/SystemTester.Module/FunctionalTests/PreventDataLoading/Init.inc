#DropDB SystemTesterEasyTest
#Application SystemTester.Web
#Application SystemTester.Win
*UseModel
*Action Log On
*ScreenCapture
*Action Navigation(PreventDataLoading.Default)

*Action New(Prevent Data Loading Object)
*FillForm
 Name = Tolis Bekiaris
*Action Save and New
*FillForm
 Name = Tolis
*Action Save and New
*FillForm
 Name = eXpandFramework
*Action Save and Close
*Action Navigation(PreventDataLoading.Default)
*CheckTable
 RowCount = 3

*Action Filter(CommonFilter)
*CheckTable 
 RowCount = 1
*Action Filter(CommonFilter)
