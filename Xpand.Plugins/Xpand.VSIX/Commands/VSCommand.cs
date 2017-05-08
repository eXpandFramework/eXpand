using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Commands{
    public abstract class VSCommand : OleMenuCommand, IDTE2Provider{
        protected VSCommand( EventHandler invokeHandler,
            CommandID commandID) : base(invokeHandler, commandID){
            var commandService = (OleMenuCommandService) VSPackage.VSPackage.Instance.GetService(typeof(IMenuCommandService));
            commandService.AddCommand(this);
        }

        protected void BindCommand(params object[] bindings) {
            if (!((object[])Command.Bindings).Any())
                Command.Bindings = bindings;
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