#IncludeFile .\Init.inc

;------------------------------Transition Permission---------------------------------
*Action Navigation(Default.Role)
*ProcessRecord 
 Name = User
*Action Permissions
*Action Permissions.New(StateMachineTransition)
*XpandFillForm
 State Machine Name = Payment
*SaveAndClose
*Action Permissions
*XpandProcessRecord Permissions
 Permission = State*
 
*XpandFillForm
 State Caption = Canceled
*SaveAndClose
*SaveAndClose

;------------------------Check that the transition is available ---------------------
*LogOff
*LogOn User

*Navigate Payment Task
*Action New
*SaveAndClose
*Navigate Payment Task
*ActionAvailable Change State(Canceled)
*ActionAvailable Change State(Paid)
*ActionAvailable Change State(Pending)

;------------------------------Check that it is not possible to to transit-----------------------
*Action Change State(Canceled)
*HandleDialog 
 Respond = OK
 Message = Permissions are not granted for transitioning to the*

;--------------------------Modify permission to hide transition---------------------
*LogOff
*LogOn Admin
*Action Navigation(Default.Role)
*ProcessRecord 
 Name = User
*Action Permissions
*XpandProcessRecord Permissions
 Permission = State*
*XpandFillForm 
 Hide = True
*SaveAndClose
*SaveAndClose
*LogOff


;------------------------Check that transition is hidden for the user---------------------------------------------
*LogOn User
*Navigate Payment Task
!ActionAvailable Change State(Canceled)
*ActionAvailable Change State(Paid)
*ActionAvailable Change State(Pending)
*LogOff

;------------------------Check that transition is not hidden for the admin---------------------------------------------
*LogOn Admin
*Navigate Payment Task
*ActionAvailable Change State(Canceled)
*ActionAvailable Change State(Paid)
*ActionAvailable Change State(Pending)



