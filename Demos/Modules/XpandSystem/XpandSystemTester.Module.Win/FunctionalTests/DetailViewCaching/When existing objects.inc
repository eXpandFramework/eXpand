*Action New(Detail View Caching Object)
*FillForm
 Year = 949
*Action Save
*Action Close
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 2

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
 Delete[0] = DetailViewCachingObject_DetailView Called 2

;Process a 3rd existing object in a listview
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = DetailViewCachingObject_DetailView Called 2
