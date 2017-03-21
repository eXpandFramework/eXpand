#IncludeFile .\Init.inc

*Navigate Popup Menu Object
*RClick 655x355
*SendKeys 
 Keys = Down;Return
*Sleep 20000
*HandleDialog  
 Caption = Go To Date
 Respond = Cancel
