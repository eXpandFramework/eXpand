using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Thumbnail.Web {
    public class ThumbnailListEditor : ListEditor,IComplexListEditor {
        public const string SelectedId = "selectedId";
        ThumbnailControl control;
        XafApplication _application;

        public ThumbnailListEditor(DictionaryNode info) : base(info) {
        }

        public override object FocusedObject { get; set; }

        public override IContextMenuTemplate ContextMenuTemplate {
            get { return null; }
        }

        public override bool AllowEdit {
            get { return false; }
            set { }
        }

        public override string[] ShownProperties
        {
            get { return Model.Columns.SortedItems.Where(nodeWrapper => nodeWrapper.VisibleIndex > -1 && !nodeWrapper.PropertyTypeInfo.IsListType).Select(wrapper => wrapper.PropertyName).ToArray(); }
        }

        public override string[] RequiredProperties
        {
            get { return Model.Columns.SortedItems.Select(wrapper => wrapper.PropertyName).ToArray(); }
        }


        public override SelectionType SelectionType {
            get { return SelectionType.TemporarySelection; }
        }


        protected override object CreateControlsCore() {
            control = new ThumbnailControl { ID = "thumbnail_control"};
            ((Page) HttpContext.Current.Handler).LoadComplete+=OnLoadComplete;
            control.RequestText+=ControlOnRequestText;
            return control;
        }

        void OnLoadComplete(object sender, EventArgs eventArgs) {
            string id = HttpContext.Current.Request.QueryString[SelectedId];
            if (id != null){
                object objectKey = ReflectionHelper.Convert(id, typeof(Guid));
                View view = _application.ProcessShortcut(new ViewShortcut(ObjectType, objectKey, Model.DetailViewId));
                _application.ShowViewStrategy.ShowView(new ShowViewParameters(view), new ShowViewSource(null, null));
            }

        }


        string GetDisplayText(IThumbNailItem pictureItem)
        {
            string text = ShownProperties.Aggregate("", (current, property) => current + (ObjectTypeInfo.FindMember(property).GetValue(pictureItem) + "<br>"));
            return text.TrimEnd("<br>".ToCharArray());
        }

        void ControlOnRequestText(object sender, RequestTextThumbnailItemEventArgs requestTextThumbnailItemEventArgs) {
            requestTextThumbnailItemEventArgs.Text = GetDisplayText(requestTextThumbnailItemEventArgs.ItemClicked);
        }


        protected override void AssignDataSourceToControl(Object dataSource) {
            if (control != null) {
                control.DataSource = ListHelper.GetList(dataSource);
            }
        }


        public override IList GetSelectedObjects() {
            var selectedObjects = new List<object>();
            if (FocusedObject != null) {
                selectedObjects.Add(FocusedObject);
            }
            return selectedObjects;
        }

        public override void Refresh() {
            if (control != null) {
                control.Refresh();
            }
        }

        public override void SynchronizeInfo() {
        }

        public void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            _application = application;
        }
    }
}