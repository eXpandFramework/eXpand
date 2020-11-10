using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;


namespace Xpand.VSIX.Commands{
    public abstract class VSCommand : OleMenuCommand, IDTE2Provider{
        private static bool _errorMessage;
        protected VSCommand( EventHandler invokeHandler,
            CommandID commandID) : base(invokeHandler, commandID){
            var commandService = (OleMenuCommandService) VSPackage.VSPackage.Instance.GetService(typeof(IMenuCommandService));
            commandService.AddCommand(this);
        }

        protected void BindCommand(DteCommand dteCommand) {
            ThreadHelper.ThrowIfNotOnUIThread();
            var instanceShowErrorsInMessageBox = OptionClass.Instance.ShowErrorsInMessageBox;
            try{
                if (dteCommand != null){
                    Command.Bindings = !string.IsNullOrWhiteSpace(dteCommand.Shortcut) ? new object[]{dteCommand.Shortcut} : new object[0];
                }
            }
            catch (Exception e){
                DTE2.LogError($"bindings:{dteCommand?.Shortcut+Environment.NewLine+e}");
                if (!_errorMessage){
                    _errorMessage = true;
                    if (instanceShowErrorsInMessageBox) {
                        MessageBox.Show($@"Errors during {GetType().Name} binding, please check your %APPDATA%\Microsoft\VisualStudio\{DTE2.Version}\ActivityLog.xml");
                    }
                }
            }
        }

        protected Command Command{
            get{
                return DTE2.Commands.Cast<Command>()
                    .First(command => command.ID == CommandID.ID && Guid.Parse(command.Guid) == CommandID.Guid);
            }
        }

        protected DTE2 DTE2 => this.DTE2();

    }

}