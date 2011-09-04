using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;
using System.Linq;

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

        public static void XafApplicationInstance(this IFaker faker, Action<XafApplication> action, Func<IList<Type>> func, Action<DetailView> viewAction, Action<Window> windowAction, params Controller[] controllers) {
            var dataSet = new DataSet();
            IObjectSpace objectSpace = ObjectSpaceInMemory.CreateNew(dataSet);
            XafApplication application = Isolate.Fake.XafApplicationInstance(func, dataSet, controllers);
            action.Invoke(application);
            object o = objectSpace.CreateObject(func.Invoke().ToList().First());
            var detailView = application.CreateDetailView(objectSpace, o);
            viewAction.Invoke(detailView);
            var window = application.CreateWindow(TemplateContext.View, controllers, true);
            windowAction.Invoke(window);
            window.SetView(detailView);

        }

        public static XafApplication XafApplicationInstance(this IFaker faker, Func<IList<Type>> func, DataSet dataSet, params Controller[] controllers) {
            var defaultSkinListGenerator = Isolate.Fake.Instance<DefaultSkinListGenerator>();
            var editorsFactory = new EditorsFactory();
#pragma warning disable 612,618
            Isolate.WhenCalled(() => editorsFactory.CreateListEditor(null, null, null)).WillReturn(new GridListEditor());
#pragma warning restore 612,618
            Isolate.Swap.AllInstances<EditorsFactory>().With(editorsFactory);

            Isolate.Swap.NextInstance<DefaultSkinListGenerator>().With(defaultSkinListGenerator);
            var application = Isolate.Fake.Instance<XafApplication>(Members.CallOriginal, ConstructorWillBe.Called);
            RegisterControllers(application, controllers);
            var xpandModuleBase = Isolate.Fake.Instance<XpandModuleBase>(Members.CallOriginal, ConstructorWillBe.Called);
            xpandModuleBase.Setup(application);
            var objectSpaceProvider = Isolate.Fake.Instance<IObjectSpaceProvider>();
            Isolate.WhenCalled(() => objectSpaceProvider.TypesInfo).WillReturn(XafTypesInfo.Instance);
            application.CreateCustomObjectSpaceProvider += (sender, args) => args.ObjectSpaceProvider = objectSpaceProvider;
            RegisterDomainComponents(application, func);
            application.Setup();
            Isolate.WhenCalled(() => application.CreateObjectSpace()).WillReturn(ObjectSpaceInMemory.CreateNew(dataSet));

            return application;
        }

        static void RegisterDomainComponents(XafApplication application, Func<IList<Type>>func) {
            func.Invoke().ToList().ForEach(type => XafTypesInfo.Instance.RegisterEntity(type));
            application.SettingUp +=
                (o, eventArgs) => func.Invoke().ToList().ForEach(type => ((ExportedTypeCollection)eventArgs.SetupParameters.DomainComponents).Add(type));
        }

        static void RegisterControllers(XafApplication application, params Controller[] controllers) {
            var methodInfo = application.GetType().GetMethod("CreateApplicationModulesManager", BindingFlags.NonPublic | BindingFlags.Instance);
            Isolate.NonPublic.WhenCalled(application, "CreateApplicationModulesManager").DoInstead(context => {
                ((ControllersManager)context.Parameters[0]).RegisterController(controllers);
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