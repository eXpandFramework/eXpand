#DropDB XpandSystemTesterEasyTest
#Application XpandSystemTester.Win

#IncludeFile Login.inc
#IncludeFile DefaultNavigation.inc

*Action Model Value(Views/SalesVolume_DetailView;DetailViewCaching;True)
*Action Model Value(Options;MinimizeOnExit;True)
*Action Model Value(Options;PromptOnExit;True)

;create new objects
*Action New(Sales Volume)
*Action Close
*HandleDialog
 Respond = No
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 1
*Action New(Sales Volume)
*Action Close
*HandleDialog
 Respond = No
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 1
*Action Log Off
#IncludeFile Login.inc
#IncludeFile DefaultNavigation.inc

;Create new object save and update
*Action New(Sales Volume)
*FillForm
 Year = 951
*Action Save and Close
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 2
*ProcessRecord 
 Year = 951
*FillForm
 Year = 952
*Action Save and Close
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 2

;Process an existing object in a listview
*Action New(Sales Volume)
*FillForm
 Year = 949
*Action Save
*Action Close
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 2

;Process a 2nd object in a listview
*Action New(Sales Volume)
*FillForm
 Year = 950
*Action Save
*Action Close
*ProcessRecord 
 Year = 950
*Action Close
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 2

;Process a 3rd existing object in a listview
*ProcessRecord 
 Year = 949
*Action Close
*CheckActionsHint
 Delete[0] = SalesVolumeMonth_DetailView Called 2

 
