using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.NCarousel;
using eXpand.Persistent.Base.General;
using eXpand.Utils.Web;
using Alignment = DevExpress.ExpressApp.Editors.Alignment;

namespace eXpand.ExpressApp.NCarousel.Web {
    public class NCarouselListEditor : ListEditor,IComplexListEditor {
        public const string     SelectedId = "selectedId";
        eXpand.NCarousel.NCarousel control;
        Session _session;
        XafApplication _application;

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
            ((Page) HttpContext.Current.Handler).LoadComplete+=OnLoadComplete;
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

        void InitCarousel() {
            DictionaryNode ncarouselNode = Model.Node.FindChildNode(NCarouselWebModule.NCarouselAttributeName);
            if (ncarouselNode!= null) {
                control.Css.AllowOverride = ncarouselNode.GetAttributeBoolValue(NCarouselWebModule.AllowOverrideAttributeName);
                control.Alignment = ncarouselNode.GetAttributeEnumValue(typeof (Alignment).Name, eXpand.NCarousel.Alignment.Horizontal);
                control.HideImages = ncarouselNode.GetAttributeBoolValue(NCarouselWebModule.HideImagesAttributeName, false);
                string container = ncarouselNode.GetAttributeValue(NCarouselWebModule.ContainerStyleAttributeName);
                if (!string.IsNullOrEmpty(container))
                    control.Css.Container = container;
                string clip = ncarouselNode.GetAttributeValue(NCarouselWebModule.ClipStyleAttributeName);
                if (!string.IsNullOrEmpty(clip))
                    control.Css.Clip = clip;
                string item = ncarouselNode.GetAttributeValue(NCarouselWebModule.ItemStyleAttributeName);
                if (!string.IsNullOrEmpty(item))
                    control.Css.Iτem = item;
                string button = ncarouselNode.GetAttributeValue(NCarouselWebModule.ButtonStyleAttributeName);
                if (!string.IsNullOrEmpty(item)) {
                    control.Css.Previous = button;
                    control.Css.Next = button;
                }
            }
        }

        protected override void AssignDataSourceToControl(Object dataSource) {
            if (control != null) {
                control.Init+=ControlOnInit;
                if (dataSource != null)
                    foreach (INCarouselItem pictureItem in ListHelper.GetList(dataSource)){
                        var args = GetDisplayText(pictureItem);
                        string text = string.Format("<a href='{0}&{3}={1}'>{2}</a>",
                                                    HttpContext.Current.Request.Url.AbsoluteUri, pictureItem.ID, args, SelectedId);            
                        control.Items.Add(new NCarouselItem(
                                              GetUrl(pictureItem),text,args));
                    }
            }
        }

        Uri GetUrl(INCarouselItem pictureItem) {
            var url = pictureItem.Image!= null? HttpContext.Current.Request.Url.AbsoluteUri + "&imageid=" + pictureItem.ID:pictureItem.ImagePath;
            if (!(string.IsNullOrEmpty(url))) 
                return new Uri(url);
            DictionaryNode ncarouselNode = Model.Node.FindChildNode(NCarouselWebModule.NCarouselAttributeName);
            if (ncarouselNode.GetAttributeBoolValue(NCarouselWebModule.UseNoImageAttributeName, true)){
                var webResourceUrl = ClientScriptProxy.Current.GetWebResourceUrl(control, GetType(), "NCarousel.Web.Resources.noimage.jpg");
                webResourceUrl =HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "") +webResourceUrl;
                return new Uri(webResourceUrl);
            }
            return null;
        }


        string GetDisplayText(INCarouselItem pictureItem)
        {
            string text = ShownProperties.Aggregate("", (current, property) => current + (ObjectTypeInfo.FindMember(property).GetValue(pictureItem) + "<br>"));
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
            _application = application;
            _session = application.ObjectSpaceProvider.CreateUpdatingReadOnlySession();
        }
    }
}