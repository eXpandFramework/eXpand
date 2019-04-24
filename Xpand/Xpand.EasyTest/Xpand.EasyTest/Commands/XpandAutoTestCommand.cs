using System;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandAutoTestCommand : Command{
        public const string Name = "XpandAutoTest";
        private const string LogonCaption = "Log On";
        


        private void TryClosePopupWindow(ICommandAdapter adapter){
            try{
                var command = new OptionalActionCommand();
                command.Parameters.MainParameter = new MainParameter("Close");
                command.Parameters.ExtraParameter = new MainParameter();
                command.Execute(adapter);
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            if (adapter.IsControlExist(TestControlType.Action, LogonCaption)){
                adapter.CreateTestControl(TestControlType.Action, LogonCaption).GetInterface<IControlAct>().Act(null);
            }
            int itemsCount = adapter.GetNavigationTestControl().GetInterface<IGridBase>().GetRowCount();
            for (int i = 0; i < itemsCount; i++){
                var testControl = GetTestControl(adapter);
                var gridBase = testControl.GetInterface<IGridBase>();
                string navigationItemName = gridBase.GetCellValue(i, new List<IGridColumn>(gridBase.Columns)[0]);
                if (!string.IsNullOrEmpty(navigationItemName)){
                    EasyTestTracer.Tracer.LogText(string.Format("EnumerateNavigationItem '{0}'", navigationItemName));
                    var controlActionItems = testControl.FindInterface<IControlActionItems>();
                    if (controlActionItems != null && controlActionItems.IsEnabled(navigationItemName)){
                        try{
                            testControl.GetInterface<IControlAct>().Act(navigationItemName);
                        }
                        catch (Exception e){
                            throw new CommandException(string.Format("The 'Navigation' action execution failed. Navigation item: {0}\r\nInner Exception: {1}",navigationItemName, e.Message), StartPosition);
                        }
                        if (adapter.IsControlExist(TestControlType.Action, "New")){
                            try{
                                adapter.CreateTestControl(TestControlType.Action, "New").FindInterface<IControlAct>().Act("");
                            }
                            catch (Exception e){
                                throw new CommandException(string.Format("The 'New' action execution failed. Navigation item: {0}\r\nInner Exception: {1}",navigationItemName, e.Message), StartPosition);
                            }
                            if (adapter.IsControlExist(TestControlType.Action, "Cancel")){
                                try{
                                    var cancelActionTestControl =adapter.CreateTestControl(TestControlType.Action, "Cancel");
                                    if (cancelActionTestControl.GetInterface<IControlEnabled>().Enabled){
                                        cancelActionTestControl.FindInterface<IControlAct>().Act(null);
                                    }
                                }
                                catch (Exception e){
                                    throw new CommandException(string.Format("The 'Cancel' action execution failed. Navigation item: {0}\r\nInner Exception: {1}",navigationItemName, e.Message), StartPosition);
                                }
                            }
                            var command = new OptionalActionCommand();
                            command.Parameters.MainParameter = new MainParameter("Yes");
                            command.Parameters.ExtraParameter = new MainParameter();
                            command.Execute(adapter);
                        }
                    }
                }
            }
        }

        private ITestControl GetTestControl(ICommandAdapter adapter){
            ITestControl testControl;
            try{
                testControl = adapter.GetNavigationTestControl();
            }
            catch (WarningException){
                TryClosePopupWindow(adapter);
                if (!adapter.IsControlExist(TestControlType.Action, "Close")){
                    var handleDialogCommand = new HandleDialogCommand();
                    handleDialogCommand.Parameters.Add(new Parameter("Respond", "No", true, StartPosition));
                    handleDialogCommand.Execute(adapter);
                }
                testControl = adapter.GetNavigationTestControl();
            }
            return testControl;
        }
    }
}