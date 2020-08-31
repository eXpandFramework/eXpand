using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic{
    [DomainLogic(typeof(IControllerStateRule))]
    public static class ControllerStateRuleDomainLogic {
        public static List<Type> Get_Controllers(IModelControllerStateRule controllerStateRule){
            return controllerStateRule.Application.ActionDesign.Controllers.Select(controller => {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(controller.Name);
                if (typeInfo == null){
                    var assemblyDefinitions = AppDomain.CurrentDomain.GetAssemblies().ToAssemblyDefinition();
                    var results = assemblyDefinitions
                        .SelectMany(definition => definition.MainModule.Types)
                        .Where(definition => definition.FullName == controller.Name)
                        .Select(definition => AppDomain.CurrentDomain.GetAssemblies()
                            .First(assembly => assembly.FullName == definition.Module.Assembly.FullName)
                            .GetType(definition.FullName))
                        .FirstOrDefault();
                    return results;
                }
                return typeInfo.Type;

            }).Where(type => type!=null).ToList();
        }

    }
}