using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.Logic {
    public interface ILogicRuleManager<TLogicRule>
    {
        List<TLogicRule> this[ITypeInfo typeInfo] { get; set; }
    }
}