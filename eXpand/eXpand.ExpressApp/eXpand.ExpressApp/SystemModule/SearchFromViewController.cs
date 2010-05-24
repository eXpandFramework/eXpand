using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelMemberSearchMode : IModelNode
    {
        [Category("eXpand")]
        [Description("Control if member will be included on full text search")]
        SearchMemberMode SearchMemberMode { get; set; }
    }
    public interface IModelPropertyEditorSearchMode : IModelNode
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelMemberSearchMode)ModelMember)", "SearchMemberMode")]
        [Description("Control if member will be included on full text search")]
        SearchMemberMode SearchMemberMode { get; set; }
    }
    public interface IModelColumnSearchMode : IModelNode
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelMemberSearchMode)ModelMember)", "SearchMemberMode")]
        [Description("Control if member will be included on full text search")]
        SearchMemberMode SearchMemberMode { get; set; }
    }

    public class SearchFromViewController : ViewController, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember, IModelMemberSearchMode>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorSearchMode>();
            extenders.Add<IModelColumn, IModelColumnSearchMode>();
        }
    }
}