;#IncludeFile .\Init.inc


*Action Navigation(Reports.Dashboard Definition)
*Action New(Dashboard Definition)
*XpandFillForm
 Name = New


*Action Designer(SetNewDashboardFields)

#IncludeFile .\DesignDashboard.inc

*SaveAndClose

*Action Navigation(Reports.Dashboard Definition)
*XpandProcessRecord
 Name = New

!ActionVisible Designer(InvalidNewDashboardXml)
!ActionVisible Designer(InvalidNewDashboardTypes)

*SaveAndClose

*LogOff
*LogOn

*Action Navigation(Dashboards.New)
*XpandCompareScreenshot DashboardViewer.png
 WinMaskRectangle = 275x257;532x425
 WebMaskRectangle = 638x332;211x816
!ActionVisible Dashboard Edit
!ActionVisible Import From XML