using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System.Linq;

namespace Xpand.ExpressApp.MapView {

    public interface IModelClassMapView : IModelNode {
        IModelMapView MapView { get; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassMapView), "ModelClass")]
    public interface IModelListViewMapView : IModelClassMapView {
    }

    public interface IModelMapView : IModelNode {

        [DataSourceProperty("AllMembers")]
        IModelMember AddressMember { get; set; }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DataSourceProperty("AllMembers")]
        IModelMember InfoWindowTextMember { get; set; }

        [Browsable(false)]
        IModelList<IModelMember> AllMembers { get; }

        bool AllowHtmlInInfoWindowText { get; set; }
    }
    [DomainLogic(typeof(IModelMapView))]
    public static class ModelMapViewDomainLogic {

        private static T GetFromListView<T>(this IModelMapView modelMapView, Func<IModelMapView, T> func)  {
            var modelListView = modelMapView.Parent as IModelListView;
            return modelListView != null ? func(((IModelClassMapView)modelListView.ModelClass).MapView) : default(T);
        }

        public static IModelMember Get_AddressMember(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.AddressMember);
        }
        public static IModelMember Get_InfoWindowText(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.InfoWindowTextMember);
        }

        public static IModelList<IModelMember> Get_AllMembers(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.AllMembers);
        }

        public static bool Get_AllowHtmlInInfoWindowText(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.AllowHtmlInInfoWindowText);
        }
    }
}
