;#IncludeFile .\Init.inc

;Create new object save and update
*Action New(Detail View Caching Object)
*FillForm
 Year = 951
*Action Save and Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1
*ProcessRecord 
 Year = 951
*FillForm
 Year = 952
*Action Save and Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1

;Process an existing object in a listview
*Action New(Detail View Caching Object)
*FillForm
 Year = 949
*Action Save
*Action Close
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1

;Process a 2nd object in a listview
*Action New(Detail View Caching Object)
*FillForm
 Year = 950
*Action Save
*Action Close
*ProcessRecord 
 Year = 950
*Action Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1

;Process a 3rd existing object in a listview
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 1
