*Action Navigation(Default.Payment Task)
*Action New(Payment Task)

!ActionAvailable Change State

*XpandFillForm
 Status = In progress
*ActionAvailable Change State(Pending)
*ActionAvailable Change State(Canceled)
*ActionAvailable Change State(Returned)
!ActionAvailable Change State(Paid)

*XpandFillForm
 Status = Completed
!ActionAvailable Change State(Pending)
!ActionAvailable Change State(Canceled)
!ActionAvailable Change State(Returned)
*ActionAvailable Change State(Paid)

*XpandFillForm
 Status = In progress
*Action Change State(Pending)
*ActionAvailable Change State(Canceled)
*ActionAvailable Change State(Returned)
!ActionAvailable Change State(Paid)

*Action Change State(Canceled)
!ActionAvailable Change State



