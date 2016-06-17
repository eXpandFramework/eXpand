using System;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{
    public class PersistentClassInfoCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customize(new PersistentAsemblyInfoCustomization());
            fixture.Customizations.Add(new PersistentClassInfoSpecimentBuilder());
        }
    }

    public class PersistentClassInfoSpecimentBuilder : ISpecimenBuilder{
        public object Create(object request, ISpecimenContext context){
            var type = request as Type;
            if (!typeof(IPersistentClassInfo).IsAssignableFrom(type)){
                return new NoSpecimen();
            }
            var assemblyInfo = context.Create<IPersistentAssemblyInfo>();
            var count = assemblyInfo.PersistentClassInfos.Count(info => info.Name.StartsWith("ClassInfo"));
            return assemblyInfo.CreateClass("ClassInfo" + count);
        }
    }
}