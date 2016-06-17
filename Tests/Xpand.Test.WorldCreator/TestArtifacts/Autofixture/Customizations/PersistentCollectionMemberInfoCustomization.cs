using System;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{
    public class PersistentCollectionMemberInfoCustomization:ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new PersistentCollectionMemberSpecimentBuilder());
        }
    }

    public class PersistentCollectionMemberSpecimentBuilder : ISpecimenBuilder {
        public object Create(object request, ISpecimenContext context) {
            var type = request as Type;
            if (!typeof(IPersistentCollectionMemberInfo).IsAssignableFrom(type)) {
                return new NoSpecimen();
            }
            var classInfo = context.Create<IPersistentClassInfo>();
            var count = classInfo.OwnMembers.Count(info => info.Name.StartsWith("ColMember"));
            var collectionClassInfo = context.Create<IPersistentClassInfo>();
            return classInfo.CreateCollection("ColMember" + count, collectionClassInfo);
        }
    }

}