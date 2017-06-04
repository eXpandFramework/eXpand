using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using MessageBox = System.Windows.MessageBox;


namespace Xpand.VSIX.Commands{
    public abstract class VSCommand : OleMenuCommand, IDTE2Provider{
        private static bool _errorMessage;
        protected VSCommand( EventHandler invokeHandler,
            CommandID commandID) : base(invokeHandler, commandID){
            var commandService = (OleMenuCommandService) VSPackage.VSPackage.Instance.GetService(typeof(IMenuCommandService));
            commandService.AddCommand(this);
        }

        protected void BindCommand(params object[] bindings) {
            try{
                if (!((object[])Command.Bindings).Any())
                    Command.Bindings = bindings;
            }
            catch (Exception e){
                DTE2.LogError($"bindings:{bindings+Environment.NewLine+e}");
                if (!_errorMessage){
                    _errorMessage = true;
                    MessageBox.Show($@"Errors during command binding, please check your %APPDATA%\Microsoft\VisualStudio\{DTE2.Version}\ActivityLog.xml");
                }
                throw;
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