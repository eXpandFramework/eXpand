using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Model;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {

    public interface IModelLogicConditionalActionState : IModelLogicContexts {
        IModelActionStateLogicRules Rules { get; }
        IModelActionContextGroup ActionContexts { get; }
    }

    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelActionContext : IModelNode {
        [DataSourceProperty("Contexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> Contexts { get; }
    }

    [DomainLogic(typeof(IModelActionContext))]
    public class ModelActionContextDomainLogic{
        public static IEnumerable<string> Get_Contexts(IModelActionContext modelActionContext){
            return modelActionContext.Application.ActionDesign.Actions.Select(action => action.Id);
        }
    }
    public class ActionContextsGroupNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    public class ActionContextNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    [ModelNodesGenerator(typeof(ActionContextsGroupNodeGenerator))]
    public interface IModelActionContextGroup : IModelNode, IModelList<IModelActionContexts> {
    }

    [ModelNodesGenerator(typeof(ActionContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelActionContexts : IModelNode, IModelList<IModelActionLink>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }


    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelActionStateLogicRules : IModelNode, IModelList<IModelActionStateRule> {
    }

}