#DropDB TreeListEditorsTesterEasyTest
#Application TreeListEditorsTester.Win
*UseModel
*ScreenCapture
*LogOn

*Action Navigation(Default.Filter By Tree Column Object)
*Action New(Filter By Tree Column Object)
*FillForm
 Name = baseobject
*Action Save and Close
*Action New(Filter By Tree Column Object Derived)
*FillForm
 Derived Property = derivepropertyvalue
 Name = derivedobject
*Action Save and Close
*ClearSelection
*Action New(Filter By Tree Column Object Derived)
*FillForm
 Derived Property = derivepropertyvalue
 Name = derivedobject2
*Action Save and Close
*SelectRecords Filter By Tree Column Object
 Columns = Name
 Row = derivedobject2
*SendKeys
 Keys = Right;Right
*Action Cell Filter
!XpandProcessRecord
 Name = baseobject
*XpandProcessRecord
 Name = derivedobject2