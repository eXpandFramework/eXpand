;#IncludeFile .\Init.inc
*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = bold font
*Action Save
*XpandCompareScreenshot DetailView.BoldFont1.png
 MaskRectangle = 55x95;51x9
*SaveAndClose

*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = 2nd bold font
*Action Save
*XpandCompareScreenshot DetailView.BoldFont2.png
 MaskRectangle = 55x95;51x13
*SaveAndClose

*Action New(Conditional Appearence Object)
*XpandFillForm
 Name = default font
*XpandCompareScreenshot DetailView.DefaultFont.png
 MaskRectangle = 55x95;51x9
*SaveAndClose

*XpandCompareScreenshot ListView.BoldFont.png
 MaskRectangle = 23x103;71x58