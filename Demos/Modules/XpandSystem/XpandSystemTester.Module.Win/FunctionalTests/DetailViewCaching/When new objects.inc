;created and not saved
*Action New(Detail View Caching Object)
*Action Close
*HandleDialog
 Respond = No
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1
*Action New(Detail View Caching Object)
*Action Close
*HandleDialog
 Respond = No
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1
*Action Log Off
#IncludeFile ..\Login.inc
*Action Navigation(Default.Detail View Caching Object)

;created and then saved and then updated
*Action New(Detail View Caching Object)
*FillForm
 Year = 951
*Action Save and Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 2
*ProcessRecord 
 Year = 951
*FillForm
 Year = 952
*Action Save and Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 2
