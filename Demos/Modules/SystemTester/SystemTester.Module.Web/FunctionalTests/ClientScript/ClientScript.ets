#IncludeFile .\Init.inc

*Navigate Client Script Object

;------------------ClientScript----------------------
*Action Client Script
*HandleDialog
 Respond = OK
*Navigate Client Script Object

;------------------ConfirmationMsg----------------------
*Action New

*Action Client Confirmation
*HandleDialog
 Respond = Yes
*XpandCheckFieldValues
 Action = ClientConfirmation
 Confirmed = True

*Action Client Confirmation
*HandleDialog
 Respond = No
*XpandCheckFieldValues
 Action = ClientConfirmation
 Confirmed = False