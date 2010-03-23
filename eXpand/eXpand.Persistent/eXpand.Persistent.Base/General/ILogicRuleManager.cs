using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace eXpand.Persistent.Base.General {
    public interface ILogicRuleManager<TLogicRule> where TLogicRule : ILogicRule {
        List<TLogicRule> this[ITypeInfo typeInfo] { get; set; }
    }
}