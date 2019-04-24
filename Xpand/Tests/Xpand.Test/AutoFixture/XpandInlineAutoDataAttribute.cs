using Ploeh.AutoFixture.Xunit2;

namespace Xpand.Test.AutoFixture{
    public class XpandInlineAutoDataAttribute:InlineAutoDataAttribute{
        public XpandInlineAutoDataAttribute(params object[] values) : this(new XpandAutoDataAttribute(),values){
        }

        public XpandInlineAutoDataAttribute(AutoDataAttribute autoDataAttribute, params object[] values) : base(autoDataAttribute, values){
        }
    }
}