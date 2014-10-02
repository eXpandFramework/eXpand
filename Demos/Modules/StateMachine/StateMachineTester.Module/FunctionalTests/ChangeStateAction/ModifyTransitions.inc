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

*XpandProcessRecord States
 Caption = Returned
*XpandFillForm
 Target Object Criteria = [Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,InProgress#
 Hide If Criteria Do Not Fit = True
*SaveAndClose

*SaveAndClose

