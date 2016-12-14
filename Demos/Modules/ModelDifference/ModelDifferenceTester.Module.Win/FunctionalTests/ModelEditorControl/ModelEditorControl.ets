
#Application ModelDifferenceTester.Win
*LogOn
*ScreenCapture

*Navigate Application Difference
*XpandProcessRecord
 Name = ModelDifferenceTester*
*Action Xml Content
*FillForm
 Xml Content = <Application Description="xmlcontent"/>

*Action Model
*LClick [ApplicationRootModelNode]
*Sleep 4000
*SendKeys
 Keys = Tab;Down;Down;Down;Down;Down;Down;Return
*Sleep 200
*SendKeys
 Modifiers = Control
 Keys = c
*Sleep 200

*CheckClipboardValue
 Value = xmlcontent
*SendKeys model

*Action Xml Content
*XpandCheckFieldValues
 Xml Content = *model*
