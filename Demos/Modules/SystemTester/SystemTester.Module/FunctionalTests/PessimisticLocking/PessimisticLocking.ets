#DropDB SystemTesterEasyTest
;#Application SystemTester.Web
#Application SystemTester.Win
*UseModel
*LogOn
*ScreenCapture


*Navigate Pessimistic Locking Object
*Action New(Pessimistic Locking Object)

;Check when object changed
*XpandFillForm 
 Name = Tolis
*SaveAndClose
*Sql Alias:SQLConnection
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where ((LockedUser IS NULL))
 Result = 1

*XpandProcessRecord
 Name = Tolis
*XpandFillForm
 Name = Tolis Bekiaris
*Sql Alias:SQLConnection
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where (NOT (LockedUser IS NULL))
 Result = 1
*SaveAndClose
*Sql Alias:SQLConnection
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
*Sql Alias:SQLConnection
 ScalarCommand = SELECT Count(*) FROM PessimisticLockingObject where (NOT (LockedUser IS NULL))
 Result = 0
