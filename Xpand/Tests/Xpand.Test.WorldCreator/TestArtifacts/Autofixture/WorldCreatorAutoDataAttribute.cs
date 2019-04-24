using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Test.AutoFixture;
using Xpand.Test.AutoFixture.Customizations;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture.Customizations;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture{
    public class WorldCreatorAutoDataAttribute : XpandAutoDataAttribute{
        public WorldCreatorAutoDataAttribute(params Type[] types) : base(types){
            Fixture.Customizations.Add(new TypeRelay(typeof(IWCTestData), typeof(WCTestData)));
            Fixture.Register(() => new Compiler(BaseSpecs.AssemblyPath));
            Fixture.Customize(new AssemblyDefintionCustomization());
            Fixture.Customize(new AssemblyManagerCustomization());
            Fixture.Customize(new PersistentAsemblyInfoCustomization());
            Fixture.Customize(new PersistentClassInfoCustomization());
            Fixture.Customize(new PersistentCoreMemberInfoCustomization());
            Fixture.Customize(new PersistentReferenceMemberInfoCustomization());
            Fixture.Customize(new PersistentCollectionMemberInfoCustomization());
        }
    }
}