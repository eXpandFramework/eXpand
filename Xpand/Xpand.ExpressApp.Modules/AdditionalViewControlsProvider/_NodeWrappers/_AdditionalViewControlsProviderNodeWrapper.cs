using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers {
    public class AdditionalViewControlsProviderNodeWrapper : NodeWrapper
    {
        public const string NodeNameAttribute = "AdditionalViewControlsProvider";

        private AdditionalViewControlsRuleWrapper _additionalViewControlsRuleWrapper;

        

        public AdditionalViewControlsProviderNodeWrapper(DictionaryNode node)
            : base(node)
        {
        }

        public AdditionalViewControlsRuleWrapper AdditionalViewControlsRuleWrapper
        {
            get
            {
                if (_additionalViewControlsRuleWrapper != null) return _additionalViewControlsRuleWrapper;
                return new ConditionalActionStateRuleNodeWrapper(Node.GetChildNode(ConditionalActionStateRuleNodeWrapper.NodeNameAttribute));
            }
            set { _additionalViewControlsRuleWrapper = value; }
        }


        public List<AdditionalViewControlsRuleWrapper> Rules
        {
            get
            {
                var rules = new List<AdditionalViewControlsRuleWrapper>();
//                addRules(ConditionalControllerStateRuleNodeWrapper.NodeNameAttribute, rules);
//                addRules(ConditionalActionStateRuleNodeWrapper.NodeNameAttribute, rules);
                return rules;
            }
        }

//        public virtual ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(
//            ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
//            where TArtifactStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper
//        {
//            ArtifactStateRuleNodeWrapper wrapper;
//            if (artifactStateRuleAttribute is ActionStateRuleAttribute)
//            {
//                wrapper =
//                    ConditionalActionStateRuleNodeWrapper.AddRule<ActionStateRuleNodeWrapper>(
//                        artifactStateRuleAttribute, typeInfo);
//            }
//            else
//            {
//                wrapper =
//                    ConditionalControllerStateRuleNodeWrapper.AddRule<ControllerStateRuleNodeWrapper>(
//                        artifactStateRuleAttribute, typeInfo);
//            }
//
//
//            return wrapper;
//        }

//        private void addRules(string controllerstaterule, List<ArtifactStateRuleNodeWrapper> rules)
//        {
//            DictionaryNode rulesNode = Node.FindChildNode(controllerstaterule);
//            if (rulesNode != null)
//            {
//                foreach (DictionaryNode node in rulesNode.ChildNodes.GetOrderedByIndex())
//                {
//                    rules.Add(
//                        (ArtifactStateRuleNodeWrapper)
//                        Activator.CreateInstance(typeof(ArtifactStateRuleNodeWrapper), new[] { node }));
//                }
//            }
//            return;
//        }

//        public IEnumerable<ArtifactStateRuleNodeWrapper> FindRules(ITypeInfo typeInfo)
//        {
//            if (typeInfo != null)
//            {
//                return ConditionalControllerStateRuleNodeWrapper.FindRules(typeInfo).Concat(ConditionalActionStateRuleNodeWrapper.FindRules(typeInfo));
//            }
//            return null;
//        }
    }
}