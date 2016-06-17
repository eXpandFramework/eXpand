using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xpand.Test.AutoFixture.Customizations;

namespace Xpand.Test.AutoFixture {
    public static class FixtureExtensions {
        public static void Remove<T>(this IList<ISpecimenBuilder> specimenBuilders) where T : ISpecimenBuilder{
            specimenBuilders.Remove(specimenBuilders.OfType<T>().First());
        }

        public static T FreezeModuleBase<T>(this IFixture fixture) where T : ModuleBase{
            var moduleBase = fixture.Freeze<T>();
//            ModuleBaseSpecimentBuilder.Setup(moduleBase);
            return moduleBase;
        }

        public static Mock<T> FreezeMock<T>(this IFixture fixture) where T : class {
            var freeze = fixture.Freeze<T>();
            return Mock.Get(freeze);
        }

    }
}
