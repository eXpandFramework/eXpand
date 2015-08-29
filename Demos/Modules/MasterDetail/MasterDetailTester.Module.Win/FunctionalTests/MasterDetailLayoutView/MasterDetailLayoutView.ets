#DropDB MasterDetailTesterEasyTest
#Application MasterDetailTester.Win
*UseModel MasterDetailLayoutView;..\CommonModel
*ScreenCapture
*LogOn

*Navigate Project
*Action New
*FillForm
 Name = eXpandFramework
*Action Contributors.New
*FillForm
 Name = Tolis Bekiaris
*Action Save and Close
*Action Save and Close
*Navigate Project

*Action Master Detail
*XpandCompareScreenshot ..\..\Images\MasterDetailLayoutView.png
 MaskRectangle = 390x188;281x281
