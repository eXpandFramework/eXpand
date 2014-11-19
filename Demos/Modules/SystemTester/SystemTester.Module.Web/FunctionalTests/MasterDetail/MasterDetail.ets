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
 WebMaskRectangle = 601x316;138x13
 ValidDiffPercentage = 3
