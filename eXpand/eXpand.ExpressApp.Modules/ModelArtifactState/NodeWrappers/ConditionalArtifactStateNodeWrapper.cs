using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers {
    public abstract class ConditionalArtifactStateNodeWrapper : NodeWrapper {
        protected ConditionalArtifactStateNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }

        public ClassInfoNodeWrapper Class {
            get {
                if (Node.Parent != null) {
                    return new ClassInfoNodeWrapper(Node.Parent);
                }
                return null;
            }
        }


        protected abstract string NodeName { get; }
        public abstract List<ArtifactStateRuleNodeWrapper> Rules { get; }

        public virtual ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(
            ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
            where TArtifactStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper {
            var nodeName = ControllerStateRuleNodeWrapper.NodeNameAttribute;
            if (artifactStateRuleAttribute is ActionStateRuleAttribute)
                nodeName = ActionStateRuleNodeWrapper.NodeNameAttribute;
            var artifactStateRuleNodeWrapper =
                (TArtifactStateRuleNodeWrapper)
                Activator.CreateInstance(typeof (TArtifactStateRuleNodeWrapper), new[] {Node.AddChildNode(nodeName)});
            artifactStateRuleNodeWrapper.EmptyCriteria = artifactStateRuleAttribute.EmptyCriteria;
            artifactStateRuleNodeWrapper.State = artifactStateRuleAttribute.State;
            artifactStateRuleNodeWrapper.NormalCriteria = artifactStateRuleAttribute.NormalCriteria;
            artifactStateRuleNodeWrapper.ViewType = artifactStateRuleAttribute.ViewType;
            artifactStateRuleNodeWrapper.Nesting = artifactStateRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.Module = artifactStateRuleAttribute.Module;
            artifactStateRuleNodeWrapper.Nesting = artifactStateRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.ID = artifactStateRuleAttribute.ID;
            artifactStateRuleNodeWrapper.TypeInfo = typeInfo;
            artifactStateRuleNodeWrapper.Description = artifactStateRuleAttribute.Description;
            artifactStateRuleNodeWrapper.ViewId = artifactStateRuleAttribute.ViewId;
            return artifactStateRuleNodeWrapper;
        }

        public override string ToString() {
            return Class != null
                       ? string.Format("{0}[{1}]", ControllerStateRulesNodeWrapper.NodeNameAttribute,
                                       Class.ClassTypeInfo.FullName)
                       : base.ToString();
        }

        public IEnumerable<ArtifactStateRuleNodeWrapper> FindRules(ITypeInfo typeInfo) {
            if (typeInfo != null) {
                foreach (ArtifactStateRuleNodeWrapper rule in Rules.Where(rule => rule.TypeInfo == typeInfo)) {
                    yield return rule;
                }
            }
        }

        protected List<ArtifactStateRuleNodeWrapper> GetRules<TControllerStateRuleNodeWrapper>()
            where TControllerStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper {
            return Node.ChildNodes.GetOrderedByIndex().Select(node => (TControllerStateRuleNodeWrapper) Activator.CreateInstance(typeof (TControllerStateRuleNodeWrapper), new[] {node})).Cast<ArtifactStateRuleNodeWrapper>().ToList();
        }
    }
}