#DropDB MasterDetailTesterEasyTest
#Application MasterDetailTester.Win
*UseModel MasterDetailRules;..\CommonModel
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
!ActionAvailable Delete
*Action Master Detail
*ActionAvailable Delete

*Action Master Detail
*XpandCompareScreenshot ..\..\Images\ExpandMasterRow.png
 MaskRectangle = 22x99;650x69




