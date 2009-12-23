using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;
using eXpand.Xpo;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator
{
    [Subject(typeof(PersistentReferenceMemberInfo))]
    public class When_is_saving_with_association_attribute_and_auto_generate_other_part_set:With_Isolations
    {
        static PersistentCollectionMemberInfo _persistentCollectionMemberInfo;
        static IArtifactHandler<PersistentReferenceMemberInfo> _artifactHandler;

        Establish context = () => {
            _artifactHandler = new TestAppLication<PersistentReferenceMemberInfo>().Setup(null,info => {
                var persistentAssemblyInfo = new PersistentAssemblyInfo(info.Session){Name = "TestAssembly"};
                info.ReferenceClassInfo = new PersistentClassInfo(info.Session) { Name = "OtherpartClass",PersistentAssemblyInfo = persistentAssemblyInfo};
                info.AutoGenerateOtherPartMember = true;
                new PersistentAssociationAttribute(info.Session){Owner = info,AssociationName = "AssociationName"};
                info.Owner=new PersistentClassInfo(info.Session){Name = "RefClass",PersistentAssemblyInfo = persistentAssemblyInfo};
            });
            _artifactHandler.WithArtiFacts(WCArtifacts).CreateDetailView().CreateFrame();
        };

        Because of = () => _artifactHandler.UnitOfWork.CommitChanges();

        It should_create_a_collection_member_with_association_attribute_to_the_other_part_class=() => {
            var otherPartClassInfo = _artifactHandler.UnitOfWork.FindObject<PersistentClassInfo>(p=>p.Name=="OtherpartClass");
            _persistentCollectionMemberInfo = otherPartClassInfo.OwnMembers.OfType<PersistentCollectionMemberInfo>().FirstOrDefault();
            _persistentCollectionMemberInfo.ShouldNotBeNull();
            var persistentAssociationAttribute = _persistentCollectionMemberInfo.TypeAttributes.OfType<PersistentAssociationAttribute>().Where(attributeInfo => attributeInfo.AssociationName=="AssociationName").FirstOrDefault();
            persistentAssociationAttribute.ShouldNotBeNull();
        };

        It should_set_collection_collectionType_same_as_RefClass =
            () => _persistentCollectionMemberInfo.CollectionTypeFullName.ShouldEqual("TestAssembly.RefClass");
    }
    [Subject(typeof(PersistentCollectionMemberInfo))]
    public class When_is_saving_with_association_attribute_and_relation_type_set_to_ManyToMany {
        It should_create_a_collection_member_with_association_attribute_to_the_other_part_class;
    }
    [Subject(typeof(PersistentCollectionMemberInfo))]
    public class When_is_saving_with_association_attribute_and_relation_type_set_to_OneToMany {
        It should_create_a_reference_member_with_association_attribute_to_the_other_part_class;
    }
}
