using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.Security;

namespace Xpand.Persistent.Base.MiddleTier {
    public class XpandWcfDataServerHelper {
        public static void AddKnownTypes() {
            var modulePath = AppDomain.CurrentDomain.SetupInformation.ApplicationName.Replace(".vshost.exe", ".exe");
            var instance = XafTypesInfo.Instance;
            var modelLoader = new ModelLoader(modulePath, instance);

            var typesInfo = TypesInfoBuilder.Create().FromModule(modulePath).Build(false);
            var xafApplication = ApplicationBuilder.Create()
            .UsingTypesInfo(s => typesInfo)
            .FromModule(modulePath)
            .FromAssembliesPath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
            .WithOutObjectSpaceProvider()
            .Build();

            modelLoader.GetMasterModel(xafApplication);
            AddKnownTypesFor<IPermissionRequest>(typesInfo);
            AddKnownTypesFor<ICustomLogonParameter>(typesInfo);
            instance.AssignAsInstance();
            xafApplication.Dispose();
            InterfaceBuilder.SkipAssemblyCleanup = true;
        }

        static void AddKnownTypesFor<T>(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.FindTypeInfo<T>().Implementors;
            var types = typeInfos.Where(info => !info.IsAbstract).Select(info => info.Type).Where(type => type.IsSerializable && !type.IsInterface);
            foreach (var type in types) {
                WcfDataServerHelper.AddKnownType(type);
            }
        }
    }
}