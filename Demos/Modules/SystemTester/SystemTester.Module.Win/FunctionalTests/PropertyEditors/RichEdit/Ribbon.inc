;-----check toolbar is visible-------
#IncludeFile ..\CheckToolBarIsVisible.inc


;-----check toolbar is not visible when not focus-------
*XpandFillForm
 Field = somevalue
#IncludeFile ..\CheckToolBarIsNotVisible.inc


;------Check the toolbar in a different tab--------
*Action Tab2
#IncludeFile ..\CheckToolBarIsNotVisible.inc
*Mouse LeftButtonClick
 MoveMouseTo = [TextField]
*Sleep 500
#IncludeFile ..\CheckToolBarIsVisible.inc

;------Check HideToolBarsAttribute--------
*Action Tab3
*Mouse LeftButtonClick
 MoveMouseTo = [TextField]
*Sleep 500
#IncludeFile ..\CheckToolBarIsNotVisible.inc