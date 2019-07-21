using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Scheduler.Win.Controllers;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using Xpand.XAF.Modules.ModelMapper.Services.Predefined;

namespace Xpand.ExpressApp.Scheduler.Win {
    [ToolboxBitmap(typeof(XpandSchedulerWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandSchedulerWindowsFormsModule : XpandModuleBase {
        public XpandSchedulerWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSchedulerModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.ExtendMap(PredefinedMap.SchedulerControl)
                .Subscribe(_ => {
                    var propertyType = _.targetInterface.GetProperty(SchedulerControlService.PopupMenusMoelPropertyName)?.PropertyType;
                    if (propertyType != null) {
                        var targetInterface = propertyType.GetInterfaces()
                            .First(type =>type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IModelList<>))
                            .GenericTypeArguments.First();
                        _.extenders.Add(targetInterface,typeof(IModelSchedulerPopupMenuItemEx));
                    }
                });
        }
    }
}