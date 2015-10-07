;#IncludeFile .\Init.inc
;---------------------EnableFilteredProperty
*Action Navigation(State Machine.State Machine)
*XpandProcessRecord 
 Name = Payment
*XpandFillForm
 Enable Filtered Property = True
*SaveAndClose


;--------------------Check if property enabled and transitions are valid----------------------------------------------
*Action Navigation(Default.Payment Task)
*Action New(Payment Task)

*XpandFillForm
 Payment Status = Canceled
!XpandFillForm
 Payment Status = New
!XpandFillForm
 Payment Status = Paid
!XpandFillForm
 Payment Status = Pending
*XpandFillForm
 Payment Status = Canceled
!ActionAvailable Change State Payment
*Action Save and New

*XpandFillForm
 Payment Status = Paid
!XpandFillForm
 Payment Status = New
!XpandFillForm
 Payment Status = Canceled
!XpandFillForm
 Payment Status = Pending
*XpandFillForm
 Payment Status = Paid
!ActionAvailable Change State Payment
*Action Save and New

*XpandFillForm
 Payment Status = Pending
*ActionAvailable Change State (Payment.Canceled)
*ActionAvailable Change State (Payment.Paid)
!ActionAvailable Change State (Payment.New)
!ActionAvailable Change State (Payment.Pending)
!XpandFillForm
 Payment Status = New
*XpandFillForm
 Payment Status = Paid
!ActionAvailable Change State Payment
*Action Save and New

*XpandFillForm
 Payment Status = Pending
!XpandFillForm
 Payment Status = New
*XpandFillForm
 Payment Status = Canceled
!ActionAvailable Change State Payment
