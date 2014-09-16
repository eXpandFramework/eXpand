;#IncludeFile .\Init.inc
*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = bold font
*Action Save
*XpandCompareScreenshot DetailView.BoldFont1.png
 WinMaskRectangle = 55x95;51x9
 WebMaskRectangle = 107x214;59x16
*SaveAndClose

*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = 2nd bold font
*Action Save
*XpandCompareScreenshot DetailView.BoldFont2.png
 WinMaskRectangle = 55x95;51x9
 WebMaskRectangle = 107x214;59x16
*SaveAndClose

*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = default font
*XpandCompareScreenshot DetailView.DefaultFont.png
 WinMaskRectangle = 55x95;51x9
 WebMaskRectangle = 107x214;59x16
*SaveAndClose
*Action Navigation(Default.Conditional Appearence Object)

*XpandCompareScreenshot ListView.BoldFont.png
 WinMaskRectangle = 26x108;75x8
 WebMaskRectangle = 112x200;91x73
 
*XpandCompareScreenshot ListView.BoldFont.png
 WinMaskRectangle = 26x128;51x9
 WebMaskRectangle = 112x200;91x73
 
*XpandCompareScreenshot ListView.BoldFont.png
 WinMaskRectangle = 26x149;57x9
 WebMaskRectangle = 112x200;91x73