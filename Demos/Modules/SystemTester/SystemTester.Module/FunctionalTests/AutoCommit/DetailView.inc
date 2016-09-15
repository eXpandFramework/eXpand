;#IncludeFile .\Init.inc
*Action Navigation(Auto Commit Object)
*DeleteAllObjects
*Action New
*XpandFillForm
 Name = ViewChanging

;-------Create new obect and change view----
*Action AutoCommit
*Navigate Auto Commit Object
*XpandProcessRecord
 Name = ViewChanging

;----Edit existing record and change view-----------
*XpandFillForm
 Name = Edited
*Action AutoCommit
*Navigate Auto Commit Object
*CheckTable
 Columns = Name
 Row = Edited

;----Edit existing record and move to next object-----------
*Action New
*XpandFillForm
 Name = 2ndObject
*Action Save
*Navigate Auto Commit Object
*XpandProcessRecord
 Name = 2ndObject
*XpandFillForm
 Name = 2ndObjectChanged
*Action Next Record
*Navigate Auto Commit Object







 