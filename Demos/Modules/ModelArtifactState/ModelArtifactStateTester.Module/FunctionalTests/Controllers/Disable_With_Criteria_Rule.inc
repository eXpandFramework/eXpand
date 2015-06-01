;#IncludeFile .\Init.inc
*Navigate Controllers Object

;EmptyCriteria
*ActionVisible Previous Record
*LogOff
*LogOn User
*Navigate Controllers Object
!ActionVisible Previous Record
*Navigate Actions Object
*ActionVisible Previous Record

;Normal Criteria
*Action New(Controllers Object)
*SaveAndClose
*ActionVisible Previous Record
*LogOff
*LogOn
