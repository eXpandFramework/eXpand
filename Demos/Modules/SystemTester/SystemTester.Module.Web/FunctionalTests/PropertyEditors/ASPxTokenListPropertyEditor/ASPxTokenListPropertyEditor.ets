;#DropDB SystemTesterEasyTest
#Application SystemTester.Web
*UseModel
*LogOn
*ScreenCapture
*Navigate PROPERTYEDITORS.ASPx Token List Property Editor Object
*Action New
*ExecuteScript
 Line1 = var testControl = TestControls.Instance.FindControl('Field', 'Childs');
 Line4 = testControl.control.SetFocus();
 Line4 = testControl.control.SetText('aaaa');
 Line1 = var testControl = TestControls.Instance.FindControl('Field', 'Name');
 Line4 = testControl.control.SetFocus();
*Action Save
*Navigate PROPERTYEDITORS.ASPx Token List Property Editor Object
*XpandProcessRecord
 CHILDS TEXT = aaaa

*ExecuteScript
 Line1 = var testControl = TestControls.Instance.FindControl('Field', 'Childs');
 Line4 = testControl.control.SetFocus();
 Line4 = testControl.control.SetText('aaaa,bbbb');
 Line1 = var testControl = TestControls.Instance.FindControl('Field', 'Name');
 Line4 = testControl.control.SetFocus();
*Action Save
*Navigate PROPERTYEDITORS.ASPx Token List Property Editor Object
*XpandProcessRecord
 CHILDS TEXT = bbbb,aaaa


