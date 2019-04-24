using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.ExpressApp.WorldCreator.System;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations{
    public class AssemblyManagerCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new TypeRelay(typeof(IAssemblyManager), typeof(AssemblyManager)));
            fixture.Register(
                () => new AssemblyManager(fixture.Create<IObjectSpace>(),
                    new CodeValidator(new Compiler(BaseSpecs.AssemblyPath), new AssemblyValidator())));
        }
    }
}