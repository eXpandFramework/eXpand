using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof (ViewContextNodeGenerator))]
// ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelViewContexts : IModelNode, IModelList<IModelViewContext>, IRule {
// ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}