using System.ComponentModel;

namespace Xpand.Persistent.Base.Logic {
    public interface IConditionalLogicRule : ILogicRule {
        [Category("Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed.")]
        string NormalCriteria { get; set; }

        [Category("Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed only used for listviews with no records.")]
        string EmptyCriteria { get; set; }
    }
}