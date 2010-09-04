using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule
{
    public interface IModelMemberSearchMode 
    {
        [Category("eXpand")]
        [Description("Control if member will be included on full text search")]
        SearchMemberMode SearchMemberMode { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberSearchMode), "ModelMember")]
    public interface IModelPropertyEditorSearchMode : IModelMemberSearchMode
    {
    }
    [ModelInterfaceImplementor(typeof(IModelMemberSearchMode), "ModelMember")]
    public interface IModelColumnSearchMode : IModelMemberSearchMode
    {

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