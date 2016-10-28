*Navigate Project
*SelectRecords
 Columns = Name
 Row = eXpandFramework

*Action Master Detail

;------check action synchronization----------------------
!ActionAvailable Delete
*Action Master Detail
*ActionAvailable Delete

;------------------check criteria-------------------------
*CheckTable
 Columns = Name,Relations
 Row = eXpandFramework,1
*ClearSelection
*SelectRecords
 Columns = Name
 Row = Project2
*Action Master Detail
*CheckTable
 Columns = Name,Relations
 Row = Project2,2
*Action Master Detail
*ClearSelection
*SelectRecords
 Columns = Name
 Row = eXpandFramework
*Action Master Detail
*CheckTable
 Columns = Name,Relations
 Row = eXpandFramework,1

 




