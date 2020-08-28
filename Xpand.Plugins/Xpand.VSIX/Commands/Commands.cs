using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE90;
using Microsoft.VisualStudio.CommandBars;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands {
    internal sealed class Commands {

        private Commands() {
            InitEasyTest();
            DropDataBaseCommand.Init();
            LoadProjectFromReferenceCommand.Init();
            ProjectConverterCommand.Init();
            SetSpecificVersion();
            XAFErrorExplorerCommand.Init();
            DisableExceptions();
            ShowModelsWindowCommand.Init();
            KillIISExpressCommand.Init();
            BuildSelectionCommand.Init();
            FindInSolutionCommand.Init();
            DuplicateLineCommand.Init();
            NavigatePreviousSubwordCommand.Init();
            NavigateNextSubwordCommand.Init();
            ShowOptionsCommand.Init();
        }

        private static void DisableExceptions(){
            if (!OptionClass.Instance.DisableExceptions){
                var exceptionsBreaks = OptionClass.Instance.Exceptions;
                var debugger = (Debugger3) DteExtensions.DTE.Debugger;
                DteExtensions.DTE.Events.DebuggerEvents.OnEnterBreakMode +=
                    (dbgEventReason reason, ref dbgExecutionAction action) =>{
                        foreach (var exceptionsBreak in exceptionsBreaks){
                            var exceptionSettings = debugger.ExceptionGroups.Item("Common Language Runtime Exceptions");
                            ExceptionSetting exceptionSetting = null;
                            try{
                                exceptionSetting = exceptionSettings.Item(exceptionsBreak.Exception);
                            }
                            catch (COMException e){
                                if (e.ErrorCode == -2147352565){
                                    exceptionSetting = exceptionSettings.NewException(exceptionsBreak.Exception, 0);
                                }
                            }
                            exceptionSettings.SetBreakWhenThrown(exceptionsBreak.Break, exceptionSetting);
                        }
                    };
            }
        }

        private void SetSpecificVersion(){
            if (OptionClass.Instance.SpecificVersion){
                DteExtensions.DTE.Events.SolutionEvents.Opened += () =>{
                    IEnumerable<IFullReference> fullReferences = null;
                    Task.Factory.StartNew(() => {
                        fullReferences = DteExtensions.DTE.Solution.GetReferences();
                    }).ContinueWith(task =>{
                        foreach (var fullReference in fullReferences){
                            fullReference.SpecificVersion = false;
                        }
                    });
                };
            }
        }

        private static void InitEasyTest(){
            var easyTestToolBar =
                ((CommandBars) DteExtensions.DTE.CommandBars).Cast<CommandBar>().FirstOrDefault(bar => bar.Name == "EasyTest");
            var commandBarControl =
                easyTestToolBar?.Controls.Cast<CommandBarControl>()
                    .FirstOrDefault(control => control.Caption == "Debug EasyTest");
            if (commandBarControl != null){
                commandBarControl.TooltipText = commandBarControl.Caption;
                commandBarControl.Caption = "D";
            }
            commandBarControl =
                easyTestToolBar?.Controls.Cast<CommandBarControl>().FirstOrDefault(control => control.Caption == "Run EasyTest");
            if (commandBarControl != null){
                commandBarControl.TooltipText = commandBarControl.Caption;
                commandBarControl.Caption = "R";
            }
            EasyTestCommand.Init();
        }

        public static Commands Instance { get; private set; }

        public static void Initialize() {
            Instance = new Commands();
        }
    }
}