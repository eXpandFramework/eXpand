using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof (ViewContextNodeGenerator))]
// ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelViewContexts : IModelNode, IModelList<IModelViewContext>, IRule {
// ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}