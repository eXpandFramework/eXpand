using System;
using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{
    public class PersistentAsemblyInfoCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new PersistentAssemblyInfoSpecimentBuilder());
        }
    }

    public class PersistentAssemblyInfoSpecimentBuilder : ISpecimenBuilder{
        public object Create(object request, ISpecimenContext context){
            var type = request as Type;
            if (!typeof(IPersistentAssemblyInfo).IsAssignableFrom(type)){
                return new NoSpecimen();
            }
            var objectSpace = context.Create<IObjectSpace>();
            var assemblyInfo = objectSpace.CreateObject<PersistentAssemblyInfo>();
            assemblyInfo.Name = "AssemblyInfo";
            return assemblyInfo;
        }
    }
}