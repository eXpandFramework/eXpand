using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.NCarousel;
using eXpand.Persistent.Base.NCarousel;
using Alignment = eXpand.NCarousel.Alignment;
using Image = System.Drawing.Image;

namespace eXpand.ExpressApp.NCarousel.Web {
    public class NCarouselListEditor : ListEditor,IComplexListEditor {
        eXpand.NCarousel.NCarousel control;
        Session _session;

        public NCarouselListEditor(DictionaryNode info) : base(info) {
        }

        public override object FocusedObject { get; set; }

        public override IContextMenuTemplate ContextMenuTemplate {
            get { return null; }
        }

        public override bool AllowEdit {
            get { return false; }
            set { }
        }

        public override string[] ShownProperties {
            get { return Model.Columns.SortedItems.Where(nodeWrapper => nodeWrapper.VisibleIndex > -1 && !nodeWrapper.PropertyTypeInfo.IsListType).Select(wrapper => wrapper.PropertyName).ToArray(); }
        }

        public override string[] RequiredProperties {
            get { return Model.Columns.SortedItems.Select(wrapper => wrapper.PropertyName).ToArray(); }
        }

        public override SelectionType SelectionType {
            get { return SelectionType.TemporarySelection; }
        }


        protected override object CreateControlsCore() {
            control = new eXpand.NCarousel.NCarousel {ID = Model.Id};
            InitCarousel();
            return control;
        }

        void InitCarousel() {
            DictionaryNode ncarouselNode = Model.Node.FindChildNode(NCarouselWebModule.NCarouselAttributeName);
            if (ncarouselNode!= null) {
                control.Alignment = ncarouselNode.GetAttributeEnumValue(typeof (Alignment).Name, Alignment.Horizontal);
                string widht = ncarouselNode.GetAttributeValue(NCarouselWebModule.WidthAttributeName);
                if (!string.IsNullOrEmpty(widht))
                    control.Width = Unit.Parse(widht);
                string height = ncarouselNode.GetAttributeValue(NCarouselWebModule.HeightAttributeName);
                if (!string.IsNullOrEmpty(height))
                    control.Height = Unit.Parse(height);
                string buttonTop = ncarouselNode.GetAttributeValue(NCarouselWebModule.ButtonPositionAttributeName);
                if (!string.IsNullOrEmpty(buttonTop))
                    control.ButtonPosition = Unit.Parse(buttonTop);
                int visibleItemsCount = ncarouselNode.GetAttributeIntValue(NCarouselWebModule.VisibleItemsCountAttributeName);
                if (visibleItemsCount>0)
                    control.VisibleItemsCount = visibleItemsCount;
            }
        }

        protected override void AssignDataSourceToControl(Object dataSource) {
            if (control != null) {
                control.Init+=ControlOnInit;
                if (dataSource != null)
                    foreach (INCarouselItem pictureItem in ListHelper.GetList(dataSource)){
                        var args = GetDisplayText(pictureItem);
                        string text = string.Format("<a href='{0}&selectedId={1}'>{2}</a>",
                                                    HttpContext.Current.Request.Url.AbsoluteUri, pictureItem.ID,args);            
                        control.Items.Add(new NCarouselItem(
                                              new Uri(HttpContext.Current.Request.Url.AbsoluteUri + "&imageid=" + pictureItem.ID),text,args));
                    }
            }
        }

        string GetDisplayText(INCarouselItem carouselItem) {
            string text = ShownProperties.Aggregate("", (current, property) => current + (ObjectTypeInfo.FindMember(property).GetValue(carouselItem) + "<br>"));
            return text.TrimEnd("<br>".ToCharArray());
        }

        void ControlOnInit(object sender, EventArgs eventArgs) {
            string id = HttpContext.Current.Request.QueryString["imageid"];
            if (id != null){
                INCarouselItem item = GetPictureItem(id);
                if (item != null && item.Image != null){
                    byte[] buffer = ImageToByteArray(item.Image);
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AppendHeader("content-length", buffer.Length.ToString());
                    HttpContext.Current.Response.ContentType = "application/x-unknown-content-type";
                    HttpContext.Current.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    HttpContext.Current.Response.End();
                }
            }
            id = HttpContext.Current.Request.QueryString["selectedId"];
            if (id != null){
                FocusedObject = GetPictureItem(id);
                OnSelectionChanged();
                OnProcessSelectedItem();       
            }
        }

        INCarouselItem GetPictureItem(string id)
        {
            object convert = ReflectionHelper.Convert(id,ObjectTypeInfo.KeyMember.MemberType);
            return _session.GetObjectByKey(ObjectType, convert) as INCarouselItem;
        }


        byte[] ImageToByteArray(Image image) {
            if (image == null) {
                throw new ArgumentNullException("image");
            }
            using (var ms = new MemoryStream()) {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
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

        }

        public override void SynchronizeInfo() {
        }

        public void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            _session = application.ObjectSpaceProvider.CreateUpdatingReadOnlySession();
        }
    }
}