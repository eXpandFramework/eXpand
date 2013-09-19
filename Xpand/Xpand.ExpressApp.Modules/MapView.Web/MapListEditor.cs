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
            mapControl.MapViewInfoNeeded+=MapControlOnMapViewInfoNeeded;
            return mapControl;
        }

        void MapControlOnMapViewInfoNeeded(object sender, MapViewInfoEventArgs mapViewInfoEventArgs) {
            var mapView = ((IModelListViewMapView) Model).MapView;
            if (mapView.AddressMember!=null) {
                var mapViewInfos = new List<MapViewInfo>();
                foreach (var obj in ((IList)MapControl.DataSource)) {
                    var mapViewInfo = new MapViewInfo();
                    var value = mapView.AddressMember.MemberInfo.GetValue(obj);
                    if (value != null) {
                        mapViewInfo.Address = value.ToString();
                        if (mapView.InfoWindowText != null) {
                            value = mapView.InfoWindowText.MemberInfo.GetValue(obj);
                            if (value != null) mapViewInfo.InfoWindowText = value.ToString();
                        }
                        mapViewInfos.Add(mapViewInfo);
                    }
                }
                mapViewInfoEventArgs.MapViewInfos = mapViewInfos;
            }
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
