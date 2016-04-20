;#IncludeFile .\Init.inc

;-----------------------------Create Admin role for Bill Statemachine-------------------------
*Action Navigation(State Machine.State Machine)
*XpandProcessRecord 
 Name = Bill
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
*XpandFillForm
 Bill Status = In dispute
!ActionAvailable Change State (Active)
!ActionAvailable Change State (InDispute)
*ActionAvailable Change State (Overdue)

*XpandFillForm
 Bill Status = Active
!ActionAvailable Change State (Active)
*ActionAvailable Change State (InDispute)
!ActionAvailable Change State (Overdue)

*XpandFillForm
 Bill Status = Overdue
!ActionAvailable Change State (Active)
!ActionAvailable Change State (InDispute)
!ActionAvailable Change State (Overdue)

*XpandFillForm
 Bill Status = Active
!ActionAvailable Change State (Active)
*ActionAvailable Change State (InDispute)
!ActionAvailable Change State (Overdue)

*XpandFillForm
 Bill Status = Overdue
!ActionAvailable Change State (Active)
!ActionAvailable Change State (InDispute)
!ActionAvailable Change State (Overdue)

*XpandFillForm
 Bill Status = In dispute
!ActionAvailable Change State (Active)
!ActionAvailable Change State (InDispute)
*ActionAvailable Change State (Overdue)

*SaveAndClose