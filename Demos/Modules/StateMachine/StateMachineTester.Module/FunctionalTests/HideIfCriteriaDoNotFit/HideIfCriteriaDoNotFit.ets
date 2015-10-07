#DropDB StateMachineTesterEasyTest
#Application StateMachineTester.Win
;#Application StateMachineTester.Web
*ScreenCapture
*LogOn

;---------------------Set HideIfCriteriaDoNotFit to all transitions
*Action Navigation(State Machine.State Machine)
*XpandProcessRecord 
 Name = Payment
*Action States

*XpandProcessRecord States
 Caption = Pending
*XpandFillForm
 Target Object Criteria = [Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,InProgress#
 Hide If Criteria Do Not Fit = True
*SaveAndClose

*XpandProcessRecord States
 Caption = Canceled
*XpandFillForm
 Target Object Criteria = [Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,InProgress#
 Hide If Criteria Do Not Fit = True
*SaveAndClose

*XpandProcessRecord States
 Caption = Paid
*XpandFillForm
 Target Object Criteria = [Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,Completed#
 Hide If Criteria Do Not Fit = True
*SaveAndClose

*SaveAndClose

;--------------------CheckChangeStateAction----------------------------------------------
*Action Navigation(Default.Payment Task)
*Action New(Payment Task)

!ActionAvailable Change State(Payment)

*XpandFillForm
 Status = In progress
*ActionAvailable Change State(Payment.Canceled)
*ActionAvailable Change State(Payment.Pending)
!ActionAvailable Change State(Payment.Paid)

*XpandFillForm
 Status = Completed
!ActionAvailable Change State(Payment.Canceled)
!ActionAvailable Change State(Payment.Pending)
*ActionAvailable Change State(Payment.Paid)





