using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ArtifactState.NodeUpdaters {
//    public abstract class ArtifactStateRulesNodeUpdater<TArtifactStateRule, TModelArtifactStateRule,TRootModelNode> : LogicRulesNodeUpdater<TArtifactStateRule, TModelArtifactStateRule,TRootModelNode>
//        where TArtifactStateRule : IArtifactStateRule
//        where TModelArtifactStateRule : IModelArtifactStateRule where TRootModelNode:IModelNode{
//
//        public override void UpdateNode(DevExpress.ExpressApp.Model.Core.ModelNode node)
//        {
//            TRootModelNode modelArtifactState = default(TRootModelNode);
//            var propertyName = modelArtifactState.GetPropertyName(ExecuteExpression());
//            if (node.Parent.Id == propertyName)
//                base.UpdateNode(node);
//        }
//
//        protected abstract Expression<Func<TRootModelNode, object>> ExecuteExpression();
//        }
}