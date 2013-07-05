using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail.Model {
    [ModelInterfaceImplementor(typeof(IMasterDetailRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelLogicMasterDetail))]
    public interface IModelMasterDetailRule : IMasterDetailRule, IModelConditionalLogicRule<IMasterDetailRule> {
        [Browsable(false)]
        IModelList<IModelListView> ChildListViews { get; }
        [Browsable(false)]
        IModelList<IModelMember> CollectionMembers { get; }
    }
}