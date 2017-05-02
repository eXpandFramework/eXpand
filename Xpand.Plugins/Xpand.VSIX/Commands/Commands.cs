using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE90;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;
using Xpand.VSIX.Wizard;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands {
    internal sealed class Commands {
        private readonly Package _package;

        private Commands(Package package) {
            if (package == null) throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null){
                InitEasyTest(commandService);
                commandService.AddCommand(new DropDataBaseCommand());
                commandService.AddCommand(new LoadProjectFromReferenceCommand());
                commandService.AddCommand(new LoadProjectFromReferenceCommand(PackageIds.cmdidLoadProjectFromreferenceTool));
                commandService.AddCommand(new ProjectConverterCommand());
                SetSpecificVersion();
                commandService.AddCommand(new XAFErrorExplorerCommand());
                DisableExceptions();


                var menuItem = new OleMenuCommand(ModelToolWindow.Show,new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidShowMEToolbox));
                menuItem.EnableForDXSolution();
                commandService.AddCommand(menuItem);

                menuItem = new OleMenuCommand(IISExpress.KillIISExpress, new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidKillIISExpress));
                commandService.AddCommand(menuItem);

                menuItem = new OleMenuCommand(OptionsPage.Show, new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidOptions));
                commandService.AddCommand(menuItem);

                menuItem = new OleMenuCommand(SolutionWizard.Show, new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidAddXpandReference));
                menuItem.ActiveForDXSolution();
                commandService.AddCommand(menuItem);

                menuItem = new OleMenuCommand(BuildSelectionCommand.Build, new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidBuildSelection));
                menuItem.EnableForSolution();
                commandService.AddCommand(menuItem);
                menuItem = new OleMenuCommand((sender, args) => FindInSolutionCommand.Find(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidFindInSolution));
                menuItem.EnableForSolution();
                commandService.AddCommand(menuItem);
            }
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

        private static void InitEasyTest(OleMenuCommandService commandService){
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
            var menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDebugEasyTest);
            var menuItem = new OleMenuCommand((sender, args) => new EasyTest().RunTest(true), menuCommandID);
            menuItem.EnableForDXSolution().EnableForActiveFile(".ets", ".inc");
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidRunEasyTest);
            menuItem = new OleMenuCommand((sender, args) => new EasyTest().RunTest(false), menuCommandID);
            menuItem.EnableForDXSolution().EnableForActiveFile(".ets", ".inc");
            commandService.AddCommand(menuItem);
        }


        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static Commands Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package) {
            Instance = new Commands(package);
        }
    }
}