using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.Logic {
    public interface ILogicInstaller {
        List<ExecutionContext> ExecutionContexts { get; }
        IModelLogic GetModelLogic(IModelApplication applicationModel);
        IModelLogic GetModelLogic();
    }
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class LogicInstallerAttribute : Attribute {
        readonly Type _installerType;

        public LogicInstallerAttribute(Type installerType) {
            _installerType = installerType;
            Guard.TypeArgumentIs(typeof(ILogicInstaller), installerType, "installerType");
        }

        public Type InstallerType {
            get { return _installerType; }
        }
    }

}