using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System.Linq;

namespace Xpand.ExpressApp.MapView {
    
    public interface IModelClassMapView:IModelNode{
        IModelMapView MapView { get; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassMapView), "ModelClass")]
    public interface IModelListViewMapView : IModelClassMapView {
    }

    public interface IModelMapView:IModelNode {
        [DataSourceProperty("AllMembers")]
        IModelMember AddressMember { get; set; }
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DataSourceProperty("AllMembers")]
        IModelMember InfoWindowText { get; set; }
        [Browsable(false)]
        IModelList<IModelMember> AllMembers { get; }
    }
    [DomainLogic(typeof(IModelMapView))]
    public class ModelMapViewDomainLogic {
        public static IModelMember Get_AddressMember(IModelMapView modelMapView) {
            var modelListView = modelMapView.Parent as IModelListView;
            return modelListView != null ? ((IModelClassMapView) modelListView.ModelClass).MapView.AddressMember : null;
        }
        public static IModelMember Get_InfoWindowText(IModelMapView modelMapView) {
            var modelListView = modelMapView.Parent as IModelListView;
            return modelListView != null ? ((IModelClassMapView) modelListView.ModelClass).MapView.InfoWindowText : null;
        }

        public static IModelList<IModelMember> Get_AllMembers(IModelMapView modelMapView) {
            var modelListView = modelMapView.Parent as IModelListView;
            return modelListView != null? new CalculatedModelNodeList<IModelMember>(modelListView.Columns.Select(column => column.ModelMember))
                       : ((IModelClass) modelMapView.Parent).AllMembers;
        }
    }
}
