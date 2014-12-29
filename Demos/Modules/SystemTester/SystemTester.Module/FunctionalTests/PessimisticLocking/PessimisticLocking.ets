#IncludeFile .\Init.inc

*Navigate Pessimistic Locking Object
*Action New(Pessimistic Locking Object)

;Check when object changed
*XpandFillForm 
 Name = Tolis
*SaveAndClose
*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS;Initial Catalog=SystemTesterEasyTest
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where ((LockedUser IS NULL))
 Result = 1

*XpandProcessRecord
 Name = Tolis
*XpandFillForm
 Name = Tolis Bekiaris
*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS;Initial Catalog=SystemTesterEasyTest
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where (NOT (LockedUser IS NULL))
 Result = 1
*SaveAndClose
*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS;Initial Catalog=SystemTesterEasyTest
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where ((LockedUser IS NULL))
 Result = 1

*SelectRecords
 Columns = Name
 Row = Tolis Bekiaris
*Action Modify
*XpandProcessRecord
 Name = Tolis Bekiaris
!XpandFillForm
 Name = ''
*Action UnLock
*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS;Initial Catalog=SystemTesterEasyTest
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where (NOT (LockedUser IS NULL))
 Result = 0
