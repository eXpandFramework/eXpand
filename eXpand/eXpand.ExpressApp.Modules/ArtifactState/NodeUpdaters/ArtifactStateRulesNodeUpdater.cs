using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ArtifactState.Logic;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ArtifactState.NodeUpdaters {
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