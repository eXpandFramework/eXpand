using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System.Linq;
using System.Collections.Generic;

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

        [DataSourceProperty("AllMembers")]
        IModelMember InfoWindowTextMember { get; set; }

        [DataSourceProperty("NumericMembers")]
        IModelMember LatitudeMember { get; set; }

        [DataSourceProperty("NumericMembers")]
        IModelMember LongitudeMember { get; set; }

        [Browsable(false)]
        IModelList<IModelMember> AllMembers { get; }

        [Browsable(false)]
        IModelList<IModelMember> NumericMembers { get; }

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
        public static IModelMember Get_InfoWindowTextMember(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.InfoWindowTextMember);
        }


        private static IModelList<IModelMember> GetMembers(IModelMapView modelMapView,  Func<IModelMember, bool> predicate)  {
            var modelListView = modelMapView.Parent as IModelListView;
            IEnumerable<IModelMember> members =
                modelListView != null ? modelListView.Columns.Select(column => column.ModelMember) : ((IModelClass)modelMapView.Parent).AllMembers;

            return new CalculatedModelNodeList<IModelMember>(members.Where(predicate));
        }

        public static IModelList<IModelMember> Get_AllMembers(IModelMapView modelMapView) {

            return GetMembers(modelMapView, m => true);
        }

        public static IModelList<IModelMember> Get_NumericMembers(IModelMapView modelMapView)   {

            return GetMembers(modelMapView, m => m.Type == typeof(float) || m.Type == typeof(double) || m.Type==typeof(decimal));
        }
        public static bool Get_AllowHtmlInInfoWindowText(IModelMapView modelMapView) {
            return modelMapView.GetFromListView(mv => mv.AllowHtmlInInfoWindowText);
        }

        public static IModelMember Get_LatitudeMember(IModelMapView modelMapView)  {
            return modelMapView.GetFromListView(mv => mv.LatitudeMember);
        }

        public static IModelMember Get_LongitudeMember(IModelMapView modelMapView)  {
            return modelMapView.GetFromListView(mv => mv.LongitudeMember);
        }
    }
}
