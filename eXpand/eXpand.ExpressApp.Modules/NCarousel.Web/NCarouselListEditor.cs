using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using System.Linq;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.NCarousel.Web {
    [ListEditor(typeof(IPictureItem))]
    public class NCarouselListEditor : ListEditor {
        public const string     SelectedId = "selectedId";
        NCarousel control;

        public NCarouselListEditor(IModelListView modelListView) : base(modelListView) {
        }

        public override object FocusedObject { get; set; }

        public override IContextMenuTemplate ContextMenuTemplate {
            get { return null; }
        }

        public override bool AllowEdit {
            get { return false; }
            set { }
        }


        public override string[] RequiredProperties {
            get { return Model.Columns.VisibleColumns.Select(column => column.PropertyName).ToArray(); }
        }

        public override SelectionType SelectionType {
            get { return SelectionType.TemporarySelection; }
        }


        protected override object CreateControlsCore() {
            control = new NCarousel {ID = Model.Id};
            control.RequestText += ControlOnRequestText;
            control.Click+=ControlOnClick;
            InitCarousel();
            return control;
        }

        void ControlOnClick(object sender, PictureItemEventArgs pictureItemEventArgs) {
            FocusedObject = pictureItemEventArgs.ItemClicked;
            OnSelectionChanged();
            OnProcessSelectedItem();
        }


        void InitCarousel() {
            IModelNCarousel modelNCarousel = ((IModelListViewNCarousel) Model).NCarousel;

            control.UseNoImage = modelNCarousel.UseNoImage;
            control.Css.AllowOverride =modelNCarousel.AllowOverride;
            control.Alignment = modelNCarousel.Alignment;
            control.HideImages = modelNCarousel.HideImages;
            string container = modelNCarousel.ContainerStyle;
            if (!string.IsNullOrEmpty(container))
                control.Css.Container = container;
            string clip = modelNCarousel.ClipStyle;
            if (!string.IsNullOrEmpty(clip))
                control.Css.Clip = clip;
            string item = modelNCarousel.ItemStyle;
            if (!string.IsNullOrEmpty(item))
                control.Css.Iτem = item;
            string button = modelNCarousel.ButtonStyle;
            if (!string.IsNullOrEmpty(item)) {
                control.Css.Previous = button;
                control.Css.Next = button;
            }
            
        }

        protected override void AssignDataSourceToControl(Object dataSource) {
            if (control != null) {
                control.DataSource = ListHelper.GetList(dataSource);
            }
        }

        void ControlOnRequestText(object sender, RequestTextPictureItemEventArgs requestTextPictureItemEventArgs) {
            requestTextPictureItemEventArgs.Text = GetDisplayText(requestTextPictureItemEventArgs.ItemClicked);
        }


        string GetDisplayText(IPictureItem pictureItem)
        {
            string text = Model.Columns.VisibleColumns.Aggregate("", (current, modelColumn) => current + (modelColumn.ModelMember.MemberInfo.GetValue(pictureItem) + "<br>"));
            return text.TrimEnd("<br>".ToCharArray());
        }


        public override IList GetSelectedObjects() {
            var selectedObjects = new List<object>();
            if (FocusedObject != null) {
                selectedObjects.Add(FocusedObject);
            }
            return selectedObjects;
        }

        public override void Refresh() {

        }

        public override void SynchronizeInfo() {
        }

    }
}