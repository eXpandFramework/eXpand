using System;
using System.IO;
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
        public static void AddKnownTypes(){
            var modulePath = AppDomain.CurrentDomain.SetupInformation.ApplicationName.Replace(".vshost.exe", ".exe");
            AddKnownTypes(modulePath);
        }

        public static void AddKnownTypes(string modulePath) {
            InterfaceBuilder.SkipAssemblyCleanup = true;
            var instance = XafTypesInfo.Instance;
            var modelLoader = new ModelLoader(modulePath, instance);
            modulePath=Path.GetFullPath(modulePath);
            var typesInfo = TypesInfoBuilder.Create().FromModule(modulePath).Build(false);
            var xafApplication = ApplicationBuilder.Create()
                .UsingTypesInfo(s => typesInfo)
                .FromModule(modulePath)
                .FromAssembliesPath(Path.GetDirectoryName(modulePath))
                .WithOutObjectSpaceProvider()
                .Build();

            modelLoader.GetMasterModel(xafApplication);
            AddKnownTypesForAll(typesInfo);
            instance.AssignAsInstance();
            xafApplication.Dispose();
            InterfaceBuilder.SkipAssemblyCleanup = true;
        }

        public static void AddKnownTypesForAll(XpandServerApplication serverApplication){
            AddKnownTypesForAll(serverApplication.TypesInfo);
        }

        private static void AddKnownTypesForAll(ITypesInfo typesInfo){
            AddKnownTypesFor<IPermissionRequest>(typesInfo);
            AddKnownTypesFor<ICustomLogonParameter>(typesInfo);
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