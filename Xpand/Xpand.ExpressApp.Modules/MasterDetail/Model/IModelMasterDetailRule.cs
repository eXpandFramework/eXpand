using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Model;
using Xpand.ExpressApp.MasterDetail.Logic;

namespace Xpand.ExpressApp.MasterDetail.Model {
    [ModelInterfaceImplementor(typeof(IMasterDetailRule), "Attribute")]
    public interface IModelMasterDetailRule : IMasterDetailRule, IModelConditionalLogicRule<IMasterDetailRule>
    {
        [Browsable(false)]
        IModelList<IModelListView> ChildListViews { get; }
        [Browsable(false)]
        IModelList<IModelMember> CollectionMembers { get; }
    }
}