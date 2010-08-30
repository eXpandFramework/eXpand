using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests {
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
        public static ExpressApp.IO.Core.TypesInfo IOTypesInfo(this IFaker faker)
        {
            Isolate.WhenCalled(() => ExpressApp.IO.Core.TypesInfo.Instance).WillReturn(Isolate.Fake.Instance<ExpressApp.IO.Core.TypesInfo>());
            var typesInfo = ExpressApp.IO.Core.TypesInfo.Instance;

            Isolate.WhenCalled(() => typesInfo.SerializationConfigurationType).WillReturn(typeof (SerializationConfiguration));
            Isolate.WhenCalled(() => typesInfo.ClassInfoGraphNodeType).WillReturn(typeof (ClassInfoGraphNode));
            

            
            return typesInfo;
        }

        public static ISecurityComplex ISecurityComplex(this IFaker faker) {
            var securityComplex = Isolate.Fake.Instance<ISecurityComplex>();
            Isolate.WhenCalled(() => SecuritySystem.Instance).WillReturn(securityComplex);
            Isolate.WhenCalled(() => securityComplex.RoleType).WillReturn(typeof (Role));
            Isolate.WhenCalled(() => securityComplex.UserType).WillReturn(typeof (User));

            Isolate.Fake.StaticMethods(typeof (SecuritySystem));
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof (User));

            var user = new User(Session.DefaultSession);
            user.Save();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);

            XafTypesInfo.Instance.RegisterEntity(securityComplex.RoleType);
            XafTypesInfo.Instance.RegisterEntity(securityComplex.UserType);
            return securityComplex;
        }
    }
}