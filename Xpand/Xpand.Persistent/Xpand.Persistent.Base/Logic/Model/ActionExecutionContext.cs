using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelActionExecutionContext : IModelNode {
        [DataSourceProperty("ExecutionContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
    }

    public class ActionExecutionContextsGroupNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    public class ActionExecutionContextNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    [ModelNodesGenerator(typeof(ActionExecutionContextsGroupNodeGenerator))]
    public interface IModelActionExecutionContextGroup : IModelNode, IModelList<IModelActionExecutionContexts> {
    }

    [ModelNodesGenerator(typeof(ActionExecutionContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelActionExecutionContexts : IModelNode, IModelList<IModelActionExecutionContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }

    
}