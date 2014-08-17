#IncludeFile .\Init.inc

*XpandProcessRecord
 Url = http://www.expandFramework.com
 Action = Edit
*XpandFillForm
 Url = aaaa
 SendKeysX = True


*Action Save and Close
*XpandCompareScreenshot HyperLinkPropertyEditor.png
 XMask = MainView.png
