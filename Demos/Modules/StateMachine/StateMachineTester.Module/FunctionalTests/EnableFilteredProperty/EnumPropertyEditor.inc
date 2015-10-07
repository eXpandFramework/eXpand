;#IncludeFile .\Init.inc

;---------------------EnableFilteredProperty
*Action Navigation(State Machine.State Machine)
*XpandProcessRecord 
 Name = Bill
*XpandFillForm
 Enable Filtered Property = True
*SaveAndClose


;--------------------Check if property enabled and transitions are valid----------------------------------------------
*Action Navigation(Default.Payment Task)
*Action New(Payment Task)

*XpandFillForm
 Bill Status = Active
*XpandFillForm
 Bill Status = In dispute
*ActionAvailable Change State(Bill.Overdue)
!ActionAvailable Change State(Bill.InDispute)
!ActionAvailable Change State(Bill.Active)
*Action Save

!XpandFillForm
 Bill Status = Active
*XpandFillForm
 Bill Status = Overdue
!ActionAvailable Change State(Bill)

!XpandFillForm
 Bill Status = Active
!XpandFillForm
 Bill Status = In dispute
*XpandFillForm
 Bill Status = Overdue
*SaveAndClose

