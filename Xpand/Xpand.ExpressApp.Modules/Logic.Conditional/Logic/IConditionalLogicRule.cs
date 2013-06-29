using System.ComponentModel;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public interface IConditionalLogicRule : ILogicRule {
        [Category("Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed.")]
        string NormalCriteria { get; set; }

        [Category("Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed only used for listviews with no records.")]
        string EmptyCriteria { get; set; }
    }
}