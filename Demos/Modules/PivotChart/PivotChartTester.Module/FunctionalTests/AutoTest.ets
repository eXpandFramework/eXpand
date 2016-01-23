#DropDB PivotChartTesterEasyTest

#Application PivotChartTester.Win
#Application PivotChartTester.Web
*ScreenCapture
*AutoTest


*Action Navigation(Reports.Analysis)
*Action New(Analysis)
*FillForm
 Name = test
 Data Type = Test Class
*Action Save
*Action Bind Analysis Data

#IncludeFile .\ExportAnalysis.inc
