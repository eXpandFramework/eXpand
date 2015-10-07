;#IncludeFile .\Init.inc
;-----------------------------Create State Machine Role Admin role -------------------------
*Action Navigation(State Machine.State Machine)
*ProcessRecord 
 Name = Payment
*XpandFillForm
 Enable Filtered Property = False
*Action Admin Roles
*Action Admin Roles.Link
*ProcessRecord 
 Name = Admin
*Action Save and Close
*Action Navigation(Default.Payment Task)
*Action New

;----------------------check that all transitions can transit---------------------

;----------------------check that all transitions can transit---------------------
*XpandFillForm
 Payment Status = New
*ActionAvailable Change State (Payment.Pending)
*ActionAvailable Change State (Payment.Canceled)
*ActionAvailable Change State (Payment.Paid)

*XpandFillForm
 Payment Status = Pending
!ActionAvailable Change State (Payment.Pending)
*ActionAvailable Change State (Payment.Canceled)
*ActionAvailable Change State (Payment.Paid)

*XpandFillForm
 Payment Status = Paid
!ActionAvailable Change State Payment

*XpandFillForm
 Payment Status = Canceled
!ActionAvailable Change State Payment

*XpandFillForm
 Payment Status = Paid
!ActionAvailable Change State Payment

*XpandFillForm
 Payment Status = Canceled
!ActionAvailable Change State Payment

*XpandFillForm
 Payment Status = Pending
!ActionAvailable Change State (Payment.Pending)
*ActionAvailable Change State (Payment.Canceled)
*ActionAvailable Change State (Payment.Paid)