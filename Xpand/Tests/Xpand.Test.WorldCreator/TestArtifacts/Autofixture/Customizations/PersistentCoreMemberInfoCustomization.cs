using System;
using System.Linq;
using DevExpress.Xpo.DB;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{
    public class PersistentCoreMemberInfoCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customize(new PersistentClassInfoCustomization());
            fixture.Customizations.Add(new PersistentCoreMemberInfoSpecimentBuilder());
        }
    }

    public class PersistentCoreMemberInfoSpecimentBuilder : ISpecimenBuilder{
        public object Create(object request, ISpecimenContext context){
            var type = request as Type;
            if (!typeof(IPersistentCoreTypeMemberInfo).IsAssignableFrom(type)){
                return new NoSpecimen();
            }
            var classInfo = context.Create<IPersistentClassInfo>();
            var count = classInfo.OwnMembers.Count(info => info.Name.StartsWith("IntMember"));
            return classInfo.CreateSimpleMember(DBColumnType.Int16, "IntMember" + count);
        }
    }
}