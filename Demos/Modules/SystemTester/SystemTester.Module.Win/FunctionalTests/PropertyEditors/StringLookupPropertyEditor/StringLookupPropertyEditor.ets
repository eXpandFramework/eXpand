#IncludeFile .\Init.inc

*Action New(String Lookup Object)
*FillForm
 Phone = (999) 000-000
*Action Save and New
*XpandCompareScreenshot lookup.png
 SendKeys = {F4}
 ValidDiffPercentage = 3
 MaskRectangle = 48x111;49x17

*XpandFillForm
 Phone = text
 SendKeys = true
!Action Save