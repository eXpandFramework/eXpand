using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers{
    public class ModelArtifactStateNodeWrapper : NodeWrapper{
        public const string NodeNameAttribute = "ModelArtifact";

        private ActionStateRulesNodeWrapper _actionStateRulesNodeWrapper;

        private ControllerStateRulesNodeWrapper _controllerStateRulesNodeWrapper;

        public ModelArtifactStateNodeWrapper(DictionaryNode node) : base(node){
        }

        public ActionStateRulesNodeWrapper ActionStateRulesNodeWrapper{
            get {
                return _actionStateRulesNodeWrapper ??new ActionStateRulesNodeWrapper(Node.GetChildNode(ActionStateRulesNodeWrapper.NodeNameAttribute));
            }
            set { _actionStateRulesNodeWrapper = value; }
        }

        public ControllerStateRulesNodeWrapper ControllerStateRulesNodeWrapper{
            get {
                return _controllerStateRulesNodeWrapper ?? new ControllerStateRulesNodeWrapper(
                                                                        Node.GetChildNode(ControllerStateRulesNodeWrapper.NodeNameAttribute));
            }
            set { _controllerStateRulesNodeWrapper = value; }
        }

        public List<ArtifactStateRuleNodeWrapper> Rules{
            get{
                var rules = new List<ArtifactStateRuleNodeWrapper>();
                addRules(ControllerStateRulesNodeWrapper.NodeNameAttribute, rules);
                addRules(ActionStateRulesNodeWrapper.NodeNameAttribute, rules);
                return rules;
            }
        }

        public virtual ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(
            ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
            where TArtifactStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper {
            
            return artifactStateRuleAttribute is ActionStateRuleAttribute
                                                       ? (ArtifactStateRuleNodeWrapper) ActionStateRulesNodeWrapper.AddRule(
                                                                                            (ActionStateRuleAttribute) artifactStateRuleAttribute, typeInfo)
                                                       : ControllerStateRulesNodeWrapper.AddRule(
                                                             (ControllerStateRuleAttribute) artifactStateRuleAttribute, typeInfo);


            
        }

        private void addRules(string controllerstaterule, List<ArtifactStateRuleNodeWrapper> rules){
            DictionaryNode rulesNode = Node.FindChildNode(controllerstaterule);
            if (rulesNode != null){
                rules.AddRange(rulesNode.ChildNodes.GetOrderedByIndex().Select(
                        node =>(ArtifactStateRuleNodeWrapper)Activator.CreateInstance(typeof (ArtifactStateRuleNodeWrapper), new[] {node})));
            }
            return;
        }

        public IEnumerable<ArtifactStateRuleNodeWrapper> FindRules(ITypeInfo typeInfo) {
            return typeInfo != null
                       ? ControllerStateRulesNodeWrapper.FindRules(typeInfo).OfType
                             <ArtifactStateRuleNodeWrapper>().Concat(
                             ActionStateRulesNodeWrapper.FindRules(typeInfo).OfType
                                 <ArtifactStateRuleNodeWrapper>())
                       : null;
        }
    }
}