;#IncludeFile .\Init.inc
*Navigate Actions Object

;EmptyCriteria
*ActionVisible Previous Record
*LogOff
*LogOn User
*Navigate Actions Object
!ActionVisible Previous Record
*Navigate Controllers Object
*ActionVisible Previous Record

;Normal Criteria
*Action New(Actions Object)
*SaveAndClose
*ActionVisible Previous Record
*LogOff
*LogOn
