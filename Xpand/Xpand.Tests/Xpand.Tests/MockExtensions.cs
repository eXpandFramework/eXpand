using System;
using System.Data;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Tests.Xpand.ExpressApp;

namespace Xpand.Tests {
    public static class MockExtensions {

        //        public static TypesInfo WCTypesInfo(this IFaker faker) {
        //            Isolate.WhenCalled(() => TypesInfo.Instance).WillReturn(Isolate.Fake.Instance<TypesInfo>());
        //            var typesInfo = TypesInfo.Instance;
        //
        //            Isolate.WhenCalled(() => typesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof (ExtendedCoreTypeMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.ExtendedCoreMemberInfoType).WillReturn(typeof (ExtendedCoreTypeMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.ExtendedReferenceMemberInfoType).WillReturn(typeof (ExtendedReferenceMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.ExtendedCollectionMemberInfoType).WillReturn(typeof (ExtendedCollectionMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentAssemblyInfoType).WillReturn(typeof (PersistentAssemblyInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentCoreTypeInfoType).WillReturn(typeof (PersistentCoreTypeMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentClassInfoInfoType).WillReturn(typeof (PersistentClassInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentCollectionInfoType).WillReturn(typeof(PersistentCollectionMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.CodeTemplateInfoType).WillReturn(typeof (CodeTemplateInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentReferenceInfoType).WillReturn(typeof (PersistentReferenceMemberInfo));
        //            Isolate.WhenCalled(() => typesInfo.CodeTemplateType).WillReturn(typeof (CodeTemplate));
        //            Isolate.WhenCalled(() => typesInfo.TemplateInfoType).WillReturn(typeof (TemplateInfo));
        //            Isolate.WhenCalled(() => typesInfo.IntefaceInfoType).WillReturn(typeof (InterfaceInfo));
        //            Isolate.WhenCalled(() => typesInfo.PersistentAssociationAttributeType).WillReturn(typeof (PersistentAssociationAttribute));
        //            Isolate.WhenCalled(() => typesInfo.PersistentDefaultClassOptionsAttributeType).WillReturn(typeof (PersistentDefaultClassOptionsAttribute));
        //            
        //            return typesInfo;
        //        }
        public static TypesInfo IOTypesInfo(this IFaker faker) {
            Isolate.WhenCalled(() => TypesInfo.Instance).WillReturn(Isolate.Fake.Instance<TypesInfo>());
            var typesInfo = TypesInfo.Instance;

            Isolate.WhenCalled(() => typesInfo.SerializationConfigurationType).WillReturn(typeof(SerializationConfiguration));
            Isolate.WhenCalled(() => typesInfo.ClassInfoGraphNodeType).WillReturn(typeof(ClassInfoGraphNode));



            return typesInfo;
        }

        public static XafApplication XafApplicationInstance(this IFaker faker, Type domaincomponentType, DataSet dataSet, params Controller[] controllers) {
            var defaultSkinListGenerator = Isolate.Fake.Instance<DefaultSkinListGenerator>();
            
            Isolate.Swap.NextInstance<DefaultSkinListGenerator>().With(defaultSkinListGenerator);
            var application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal, ConstructorWillBe.Called);
            RegisterControllers(application, controllers);
            var xpandModuleBase = Isolate.Fake.Instance<XpandModuleBase>(Members.CallOriginal, ConstructorWillBe.Called);
            xpandModuleBase.Setup(application);
//            Isolate.WhenCalled(() => XpandModuleBase.Application).WillReturn(application);
            var objectSpaceProvider = Isolate.Fake.Instance<IObjectSpaceProvider>();
            Isolate.WhenCalled(() => objectSpaceProvider.TypesInfo).WillReturn(XafTypesInfo.Instance);
            application.CreateCustomObjectSpaceProvider += (sender, args) => args.ObjectSpaceProvider = objectSpaceProvider;
            RegisterDomainComponents(application, domaincomponentType);
            application.Setup();
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(ObjectSpaceInMemory.CreateNew(dataSet));
            
            return application;
        }

        static void RegisterDomainComponents(XafApplication application, Type domaincomponentType) {
            XafTypesInfo.Instance.RegisterEntity(domaincomponentType);
            application.SettingUp +=
                (o, eventArgs) => ((BusinessClassesList) eventArgs.SetupParameters.DomainComponents).Add(typeof (PessimisticLockObject));
        }

        static void RegisterControllers(XafApplication application, params Controller[] controllers) {
            var methodInfo = application.GetType().GetMethod("CreateApplicationModulesManager",BindingFlags.NonPublic|BindingFlags.Instance);
            Isolate.NonPublic.WhenCalled(application,"CreateApplicationModulesManager").DoInstead(context => {
                ((ControllersManager) context.Parameters[0]).RegisterController(controllers);
                return methodInfo.Invoke(application, new[] { context.Parameters[0] });
            });
        }


        public static ISecurityComplex ISecurityComplex(this IFaker faker) {
            var securityComplex = Isolate.Fake.Instance<ISecurityComplex>();
            Isolate.WhenCalled(() => SecuritySystem.Instance).WillReturn(securityComplex);
            Isolate.WhenCalled(() => securityComplex.RoleType).WillReturn(typeof(Role));
            Isolate.WhenCalled(() => securityComplex.UserType).WillReturn(typeof(User));

            Isolate.Fake.StaticMethods(typeof(SecuritySystem));
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));

            var user = new User(Session.DefaultSession);
            user.Save();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);

            XafTypesInfo.Instance.RegisterEntity(securityComplex.RoleType);
            XafTypesInfo.Instance.RegisterEntity(securityComplex.UserType);
            return securityComplex;
        }
    }
}