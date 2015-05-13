;#IncludeFile .\Init.inc

*Action Navigation(Reports.Dashboard Definition)
*Action New(Dashboard Definition)
*XpandFillForm
 Name = New
 
*Action Set New Dashboard Fields

#IncludeFile .\DesignDashboard.inc

*SaveAndClose

*Action Navigation(Reports.Dashboard Definition)
*XpandProcessRecord
 Name = New

!ActionVisible Invalid New Dashboard Xml
!ActionVisible Invalid New Dashboard Types

*SaveAndClose

*LogOff
*LogOn

*Action Navigation(Dashboards.New)
*XpandCompareScreenshot DashboardViewer.png
 WinMaskRectangle = 275x257;532x425
 WebMaskRectangle = 258x320;483x370
!ActionVisible Dashboard Edit
!ActionVisible Import From XML