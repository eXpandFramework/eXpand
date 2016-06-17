using Ploeh.AutoFixture.Xunit2;
using Xpand.Test.AutoFixture;

namespace Xpand.Test.WorldCreator.TestArtifacts.Autofixture{
    public class WorldCreatorInlineAutoDataAttribute:XpandInlineAutoDataAttribute{
        public WorldCreatorInlineAutoDataAttribute(params object[] values) : this(new WorldCreatorAutoDataAttribute(),values){
        }

        public WorldCreatorInlineAutoDataAttribute(AutoDataAttribute autoDataAttribute, params object[] values) : base(autoDataAttribute, values){
        }
    }
}