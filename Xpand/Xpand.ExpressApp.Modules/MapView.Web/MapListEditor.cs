using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.MapView.Web {
    [ListEditor(typeof(object), false)]
    public class MapListEditor : ListEditor, IXpandListEditor {
        public MapListEditor(IModelListView model)
            : base(model) {

        }

        public event EventHandler<ViewControlCreatedEventArgs> ViewControlsCreated;

        protected override void AssignDataSourceToControl(object dataSource) {
            if (Control != null)
                ((MapControl)Control).DataSource = dataSource;
        }

        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate {
            get { return null; }
        }

        protected override object CreateControlsCore() {
            var mapControl = new MapControl();
            mapControl.FocusedIndexChanged += (s, e) => OnFocusedObjectChanged();
            mapControl.MapViewInfoNeeded += MapControlOnMapViewInfoNeeded;
            mapControl.PerformCallbackOnMarker = IsMasterDetail;
            mapControl.AllowHtmlInInfoText = ModelMapView.AllowHtmlInInfoWindowText;
            return mapControl;
        }

        private bool IsMasterDetail {
            get { return Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView; }
        }

        private IModelMapView ModelMapView {
            get { return ((IModelListViewMapView)Model).MapView; }
        }

        private T GetMemberValue<T>(object obj, IModelMember modelMember) {
            if (modelMember == null) return default(T);
            var value = modelMember.MemberInfo.GetValue(obj);
            if (value != null)
                return (T)value;
            else
                return default(T);
        }

        private string GetMemberValueToString(object obj, IModelMember modelMember) {
            object value = GetMemberValue<object>(obj, modelMember);
            return value != null ? value.ToString() : null;
        }
        
        void MapControlOnMapViewInfoNeeded(object sender, MapViewInfoEventArgs mapViewInfoEventArgs) {
            var mapView = ModelMapView;
            var mapViewInfos = new List<MapViewInfo>();
            foreach (var obj in ((IList)MapControl.DataSource)) {
                var mapViewInfo = new MapViewInfo();
                string address = GetMemberValueToString(obj, mapView.AddressMember);
                if (!string.IsNullOrEmpty(address)) {
                    mapViewInfo.Address = address;
                }
                
                mapViewInfo.InfoWindowText  = GetMemberValueToString(obj, mapView.InfoWindowTextMember);
                
                mapViewInfo.Latitude = GetMemberValue<decimal?>(obj, mapView.LatitudeMember);
                mapViewInfo.Longitude = GetMemberValue<decimal?>(obj, mapView.LongitudeMember);
                mapViewInfos.Add(mapViewInfo);
            }
            mapViewInfoEventArgs.MapViewInfos = mapViewInfos;
        }

        public override IList GetSelectedObjects() {
            return new object[0];
        }

        public override void Refresh() {

        }

        public override SelectionType SelectionType {
            get { return SelectionType.FocusedObject; }
        }

        public void NotifyViewControlsCreated(XpandListView listView) {
            if (ViewControlsCreated != null)
                ViewControlsCreated(this, new ViewControlCreatedEventArgs(listView.IsRoot));
        }


        private MapControl MapControl {
            get { return (MapControl)Control; }
        }
        public override object FocusedObject {
            get {
                return MapControl != null ? MapControl.FocusedObject : null;
            }
            set {
            }
        }
    }
}
