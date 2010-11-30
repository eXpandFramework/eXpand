using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Thumbnail.Web {
    [ListEditor(typeof(IPictureItem))]
    public class ThumbnailListEditor : ListEditor {
        public const string SelectedId = "selectedId";
        ThumbnailControl control;
        

        public ThumbnailListEditor(IModelListView modelListView) : base(modelListView) {
        }

        public override object FocusedObject { get; set; }

        public override IContextMenuTemplate ContextMenuTemplate {
            get { return null; }
        }

        public override bool AllowEdit {
            get { return false; }
            set { }
        }


        public override string[] RequiredProperties
        {
            get { return Model.Columns.GetVisibleColumns().Select(wrapper => wrapper.PropertyName).ToArray(); }
        }


        public override SelectionType SelectionType {
            get { return SelectionType.TemporarySelection; }
        }


        protected override object CreateControlsCore() {
            IModelThumbnailWeb modelThumbnailWeb = ((IModelListViewThumbnailWeb) Model).ThumbnailWeb;
            control = new ThumbnailControl {
                                               ID = "thumbnail_control",
                                               DisplayStyle =modelThumbnailWeb.DisplayStyle,
                                               HideImages = modelThumbnailWeb.HideImages

                                           };
            control.Click+=ControlOnClick;
            control.RequestText+=ControlOnRequestText;
            return control;
        }

        void ControlOnClick(object sender, PictureItemEventArgs pictureItemEventArgs) {
            FocusedObject = pictureItemEventArgs.ItemClicked;
            OnSelectionChanged();
            OnProcessSelectedItem();
        }



        string GetDisplayText(IPictureItem pictureItem)
        {
            string text = Model.Columns.GetVisibleColumns().Aggregate("", (current, modelColumn) => current + (ObjectTypeInfo.FindMember(modelColumn.PropertyName).GetValue(pictureItem) + "<br>"));
            return text.TrimEnd("<br>".ToCharArray());
        }

        void ControlOnRequestText(object sender, RequestTextPictureItemEventArgs requestTextPictureItemEventArgs) {
            requestTextPictureItemEventArgs.Text = GetDisplayText(requestTextPictureItemEventArgs.ItemClicked);
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
        public override void SynchronizeModel() {
            
        }

    }
}