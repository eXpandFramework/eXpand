using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.TypeConverters;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public sealed class ObjectViewRuleAttribute : LogicRuleAttribute, IObjectViewRule {
        public ObjectViewRuleAttribute(string id, string normalCriteria, string emptyCriteria, string objectView)
            : base(id, normalCriteria, emptyCriteria) {
            ObjectView = objectView;
        }

        public string ObjectView { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelObjectView IObjectViewRule.ObjectView { get; set; }
    }
}
