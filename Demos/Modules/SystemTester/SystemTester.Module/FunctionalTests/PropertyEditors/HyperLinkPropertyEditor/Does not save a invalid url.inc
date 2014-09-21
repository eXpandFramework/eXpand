;#IncludeFile .\Init.inc
*Action New(Hyper Link Object)
*Action Toggle Navigation
*FocusWindow
*XpandFillForm
 Url = aaaa
 SendKeys = True
*Action Save and Close
*XpandCompareScreenshot HyperLinkPropertyEditor.png
 MaskRectangle = 39x94;12x12
 ToggleNavigation = False
*Action Toggle Navigation

