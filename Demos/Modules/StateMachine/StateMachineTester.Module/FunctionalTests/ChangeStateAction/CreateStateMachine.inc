*Action Navigation(State Machine.State Machine)
*Action New(State Machine)

*XpandFillForm
 Name = Payment
 Target Object Type = Payment Task
 Active = True
 State Property Name = PaymentStatus

*ExecuteEditorAction Start State
*Action New
*XpandFillForm
 Marker = New
*Action Transitions
*Action Transitions.New
*ExecuteEditorAction Target State

*Action New
*XpandFillForm
 Marker = Canceled
*SaveAndClose
*SaveAndClose
*Action Transitions.New
*ExecuteEditorAction Target State

*Action New
*XpandFillForm
 Marker = Paid
*SaveAndClose
*SaveAndClose
*Action Transitions.New
*ExecuteEditorAction Target State

*Action ObjectsCreation.New
*XpandFillForm
 Marker = Pending
*SaveAndClose
*SaveAndClose
*Action Transitions.New
*ExecuteEditorAction Target State

*Action New
*XpandFillForm
 Marker = Returned
*SaveAndClose
*SaveAndClose
*SaveAndClose

*Action States
*XpandProcessRecord States
 Caption = Pending
 
*Action Transitions
*Action Transitions.New
*FillForm
 Target State = Canceled
*SaveAndClose
*Action Transitions.New
*XpandFillForm
 Target State = Paid
*SaveAndClose
*Action Transitions.New
*XpandFillForm
 Target State = Returned
*SaveAndClose
*SaveAndClose
*SaveAndClose