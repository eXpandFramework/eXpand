using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace eXpand.Persistent.Base.General {
    public interface IModelRuleManager<TModelRule> where TModelRule : IModelRule {
        List<TModelRule> this[ITypeInfo typeInfo] { get; set; }
    }
}