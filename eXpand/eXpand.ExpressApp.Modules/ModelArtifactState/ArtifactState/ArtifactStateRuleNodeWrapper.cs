using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState {
    public abstract class ArtifactStateRuleNodeWrapper : ModelRuleNodeWrapper, IArtifactRule {
        public const string ModuleAttribute = "Module";


        protected ArtifactStateRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode) {
        }

        public abstract string NodeName { get; }

        #region IArtifactRule Members
        public string Module {
            get { return Node.GetAttributeValue(ModuleAttribute); }
            set { Node.SetAttribute(ModuleAttribute, value); }
        }
        #endregion

    }
}