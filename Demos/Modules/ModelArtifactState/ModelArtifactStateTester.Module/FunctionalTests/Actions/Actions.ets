#IncludeFile .\Init.inc

*Navigate Actions Object

;Hide_Actions rule
!ActionVisible Delete
!ActionVisible Refresh
!ActionVisible Next Record

;Hide_With_Criteria rule
!ActionVisible Previous Record
*Action New(Actions Object)
*SaveAndClose
*ActionVisible Previous Record

;Hide_From_Modules rule
!ActionVisible Test 1 Action
!ActionVisible Test 2 Action

