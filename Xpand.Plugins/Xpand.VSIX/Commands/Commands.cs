//------------------------------------------------------------------------------
// <copyright file="RunEasyTest.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE90;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.Options;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class Commands{
        

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private static readonly Events _dteEvents=DteExtensions.DTE.Events;
        private readonly CommandEvents _commandEvents= _dteEvents.CommandEvents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Commands" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private Commands(Package package){
            if (package == null) throw new ArgumentNullException(nameof(package));
            
            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null){
                var menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDebugEasyTest);
                var menuItem = new OleMenuCommand((sender, args) =>{
                    foreach (var cbar in ((CommandBars)DteExtensions.DTE.CommandBars).Cast<CommandBar>().Where(bar => bar.Name == "EasyTest")) {
                        var cbarParent = cbar.Parent;
                    }
                    foreach (var cbar in ((CommandBars)DteExtensions.DTE.CommandBars).Cast<CommandBar>().Where(bar => bar.Name.Contains("Sample"))) {
                        var cbarParent = cbar.Parent;
                    }
                    //                    new EasyTest().RunTest(true)
                }, menuCommandID);
                menuItem.EnableForDXSolution().EnableForActiveFile(".ets",".inc");
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidRunEasyTest);
                menuItem = new OleMenuCommand((sender, args) => new EasyTest().RunTest(false), menuCommandID);
                menuItem.EnableForDXSolution().EnableForActiveFile(".ets",".inc");
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDropDatabase);
                menuItem = new OleMenuCommand((sender, args) => DropDataBase.Drop(), menuCommandID);
                menuItem.EnableForConfigFile();
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidLoadProjectFromreference);
                menuItem = new OleMenuCommand((sender, args) => LoadProjectFromReference.LoadProjects(), menuCommandID);
                menuItem.EnableForAssemblyReferenceSelection();
                commandService.AddCommand(menuItem);
                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidLoadProjectFromreferenceTool);
                menuItem = new OleMenuCommand((sender, args) => LoadProjectFromReference.LoadProjects(), menuCommandID);
                menuItem.EnableForAssemblyReferenceSelection();
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidProjectConverter);
                menuItem = new OleMenuCommand((sender, args) => ProjectConverter.Convert(), menuCommandID);
                menuItem.EnableForDXSolution();
                commandService.AddCommand(menuItem);

                _dteEvents.SolutionEvents.Opened += () =>{
                    if (OptionClass.Instance.SpecificVersion){
                        IEnumerable<IFullReference> fullReferences = null;
                        Task.Factory.StartNew(() => {fullReferences = DteExtensions.DTE.GetReferences();
                        }).ContinueWith(task =>{
                            foreach (var fullReference in fullReferences){
                                fullReference.SpecificVersion = false;
                            }
                        });
                    }
                };

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidExploreXAFErrors);
                menuItem = new OleMenuCommand((sender, args) => XAFErrorExplorer.Explore(), menuCommandID);
                commandService.AddCommand(menuItem);

                if (!OptionClass.Instance.DisableExceptions) {
                    var exceptionsBreaks = OptionClass.Instance.Exceptions;
                    var debugger = (Debugger3)DteExtensions.DTE.Debugger;
                    _dteEvents.DebuggerEvents.OnEnterBreakMode +=
                        (dbgEventReason reason, ref dbgExecutionAction action) =>{
                            foreach (var exceptionsBreak in exceptionsBreaks){
                                var exceptionSettings =debugger.ExceptionGroups.Item("Common Language Runtime Exceptions");
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

                menuCommandID = new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidShowMEToolbox);
                menuItem = new OleMenuCommand((sender, args) => _package.ShowToolWindow<ModelToolWindow>(), menuCommandID);
                commandService.AddCommand(menuItem);
            }
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
        public static void Initialize(Package package){
            Instance = new Commands(package);
        }

    }
}