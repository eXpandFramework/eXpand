#IncludeFile .\Init.inc

*Navigate Auto Refresh
*CheckTable
 RowCount = 0
*Sleep 3500
*CheckTable
 RowCount = 1
*Action Close
*Sleep 3500
*Navigate Auto Refresh
*CheckTable
 RowCount = 1

;-------------------When Document Manager Is Active------------------
*Action New
*Sleep 3500
*Navigate Auto Refresh
*CheckTable
 RowCount = 1