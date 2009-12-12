using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.WorldCreator
{


    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CoreMembers_that_exist_already:With_Isolations {
        static IArtifactHandler<ExtendedCoreTypeMemberInfo> _artifactHandler;

        static TypesInfo _typesInfo;

        Establish context = () =>
        {
            _typesInfo = TypesInfo.Instance;
            _artifactHandler = new TestAppLication<ExtendedCoreTypeMemberInfo>().Setup(null, memberInfo => {
                memberInfo.DataType = XPODataType.String;
                memberInfo.Name = "UserName";
                memberInfo.Owner = typeof (User);
                memberInfo.TypeAttributes.Add(new PersistentSizeAttribute(memberInfo.Session));                
            });
            _artifactHandler.UnitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(_artifactHandler.UnitOfWork, _typesInfo);


        It should_not_throw_any_exceptions = () => XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("UserName");
    }
    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CoreMembers :With_Isolations
    {
        static TypesInfo _typesInfo;
        static UnitOfWork _unitOfWork;
        static XPMemberInfo memberInfo;


        Establish context = () => {
            _typesInfo = TypesInfo.Instance;
            var artifactHandler = new TestAppLication<ExtendedCoreTypeMemberInfo>().Setup(null,typeMemberInfo => {
                typeMemberInfo.DataType=XPODataType.Boolean;
                typeMemberInfo.Name = "Test";
                typeMemberInfo.Owner = typeof (User);
                typeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(typeMemberInfo.Session));                
            });
            _unitOfWork = artifactHandler.UnitOfWork;
            _unitOfWork.CommitChanges();
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(_unitOfWork, _typesInfo);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (SizeAttribute)).ShouldNotBeNull();
    }
    [Subject(typeof(ExistentTypesMemberCreator))]
    public class When_Creating_ExistentTypes_ReferenceMembers : With_Isolations
    {
        static TypesInfo _typesInfo;
        static UnitOfWork _unitOfWork;
        static XPMemberInfo memberInfo;

        Establish context = () => {
            _typesInfo = TypesInfo.Instance;
            var artifactHandler = new TestAppLication<ExtendedReferenceMemberInfo>().Setup(null, referenceMemberInfo => {
                referenceMemberInfo.Name = "Test";
                referenceMemberInfo.Owner = typeof (User);
                referenceMemberInfo.ReferenceType = typeof (Role);
                referenceMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(referenceMemberInfo.Session));
            });
            _unitOfWork = artifactHandler.UnitOfWork;
            _unitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(_unitOfWork, _typesInfo);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (SizeAttribute)).ShouldNotBeNull();
    }
    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CollectionMembers : With_Isolations
    {
        static UnitOfWork _unitOfWork;
        static TypesInfo _typesInfo;
        static XPMemberInfo memberInfo;

        Establish context = () => {
            _typesInfo = TypesInfo.Instance;
            var artifactHandler = new TestAppLication<ExtendedCollectionMemberInfo>().Setup(null, collectionMemberInfo => {
                collectionMemberInfo.Name = "Test";
                collectionMemberInfo.Owner = typeof (User);
                collectionMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(collectionMemberInfo.Session));                
            });
            _unitOfWork = artifactHandler.UnitOfWork;
            _unitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(_unitOfWork, _typesInfo);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (SizeAttribute)).ShouldNotBeNull();
    }

}
