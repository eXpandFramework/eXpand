using eXpand.ExpressApp.Logic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic.Conditional {
    public interface IConditionalLogicRule : ILogicRule
    {
        /// <summary>
        /// Criteria to apply when show DetailView or filled ListView 
        /// </summary>
        string NormalCriteria { get; set; }

        /// <summary>
        /// Criteria to apply when show ListView empty
        /// </summary>
        string EmptyCriteria { get; set; }
    }
}