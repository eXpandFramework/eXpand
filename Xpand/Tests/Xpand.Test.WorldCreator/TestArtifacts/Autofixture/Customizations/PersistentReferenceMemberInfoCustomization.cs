using System;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{

    public class PersistentReferenceMemberInfoCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customize(new PersistentClassInfoCustomization());
            fixture.Customizations.Add(new ReferenceMemberSpecimentBuilder());
        }
    }

    public class ReferenceMemberSpecimentBuilder : ISpecimenBuilder{
        public object Create(object request, ISpecimenContext context){
            var type = request as Type;
            if (!typeof(IPersistentReferenceMemberInfo).IsAssignableFrom(type)){
                return new NoSpecimen();
            }
            var classInfo = context.Create<IPersistentClassInfo>();
            var count = classInfo.OwnMembers.Count(info => info.Name.StartsWith("RefMember"));
            return classInfo.CreateReferenceMember("RefMember" + count, classInfo);
        }
    }
}