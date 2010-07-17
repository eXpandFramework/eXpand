using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Model;
using eXpand.ExpressApp.MasterDetail.Logic;

namespace eXpand.ExpressApp.MasterDetail.Model {
    [ModelInterfaceImplementor(typeof(IMasterDetailRule), "Attribute")]
    public interface IModelMasterDetailRule : IMasterDetailRule, IModelConditionalLogicRule<IMasterDetailRule>
    {
        [Browsable(false)]
        IModelList<IModelListView> ChildListViews { get; }
    }
}