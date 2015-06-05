#IncludeFile .\Init.inc

*Navigate Master
*Action New(Master)
*XpandFillForm
 Name = Master
*SaveAndClose
*Navigate Master
*XpandCompareScreenshot MasterDetail.png
 WebMaskRectangle = 115x174;651x158

*Action New Detail
*XpandFillForm
 Name = Detail
*SaveAndClose
*Navigate Master
*XpandCompareScreenshot MasterDetail.png
 WebMaskRectangle = 562x335;184x11
 ValidDiffPercentage = 7

 