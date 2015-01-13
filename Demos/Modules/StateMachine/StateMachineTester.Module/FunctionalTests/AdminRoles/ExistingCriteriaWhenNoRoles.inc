*Navigate State Machine
*XpandProcessRecord
 Name = Test

*Action Admin Roles
*Action Admin Roles.Link
*ProcessRecord 
 Name = User

*Action States
*ProcessRecord States
 Caption = Completed
*FillForm
 Target Object Criteria = IsCurrentUserInRole('User')
*SaveAndClose
*SaveAndClose

*Action Navigation(Default.Test Task)
*Action New(Test Task)
*Action Change State(NotStartedInProgress)
*Action Change State(InProgressCompleted)


*CheckValidationResult
 Info = *Completed*
 
*OptionalAction Close
*Action Log Off
*HandleDialog
 Respond = No