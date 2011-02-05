using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using Xpand.NCarousel;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Web;

namespace Xpand.ExpressApp.NCarousel.Web {
    public class NCarousel : Xpand.NCarousel.NCarousel, IPostBackEventHandler {
        public IList DataSource { get; set; }
        #region IPostBackEventHandler Members
        public void RaisePostBackEvent(string eventArgument) {
            foreach (IPictureItem item in DataSource) {
                if (item.ID == eventArgument) {
                    OnClick(new PictureItemEventArgs(item));
                    break;
                }
            }
        }
        #endregion

        public event EventHandler<PictureItemEventArgs> Click;

        public void OnClick(PictureItemEventArgs e) {
            EventHandler<PictureItemEventArgs> handler = Click;
            if (handler != null) handler(this, e);
        }
        IPictureItem FindItemByID(string id) {
            if (DataSource == null) return null;
            return DataSource.Cast<IPictureItem>().FirstOrDefault(item => item.ID == id);
        }
        protected override void CreateChildControls() {
            Refresh();
            base.CreateChildControls();
        }

        public void Refresh() {
            Controls.Clear();
            if (Page != null) {
                foreach (IPictureItem pictureItem in ListHelper.GetList(DataSource)) {
                    var requestTextPictureItemEventArgs = new RequestTextPictureItemEventArgs(pictureItem);
                    OnRequestText(requestTextPictureItemEventArgs);
                    var displayText = requestTextPictureItemEventArgs.Text;
                    string text = string.Format(@"<a href=Javascript:" + GetDataItemFunctionBody(pictureItem.ID) + @">{2}</a>",
                        HttpContext.Current.Request.Url.AbsoluteUri, pictureItem.ID, displayText);
                    Items.Add(new NCarouselItem(GetUrl(pictureItem), text, displayText));
                }

            }
        }
        private string GetDataItemFunctionBody(string key) {
            var options = new PostBackOptions(this, key);
// ReSharper disable PossibleNullReferenceException
            return ((Page) ((WebWindowTemplateHttpHandler) HttpContext.Current.Handler).ActualHandler).ClientScript.GetPostBackEventReference(options).Replace("'", "&#39;");
// ReSharper restore PossibleNullReferenceException
        }
        public event EventHandler<RequestTextPictureItemEventArgs> RequestText;

        public void OnRequestText(RequestTextPictureItemEventArgs e) {
            EventHandler<RequestTextPictureItemEventArgs> handler = RequestText;
            if (handler != null) handler(this, e);
        }

        [DefaultValue(false)]
        public bool UseNoImage {
            get {
                object useNoImage = ViewState["UseNoImage"];
                return useNoImage != null && (bool)useNoImage;
            }
            set { ViewState["UseNoImage"] = value; }
        }

        Uri GetUrl(IPictureItem pictureItem) {
            var url = pictureItem.Image != null ? HttpContext.Current.Request.Url.AbsoluteUri + "&imageid=" + pictureItem.ID : pictureItem.ImagePath;
            if (!(string.IsNullOrEmpty(url)))
                return new Uri(url);
            if (UseNoImage) {
                var webResourceUrl = ClientScriptProxy.Current.GetWebResourceUrl(GetType(), "Xpand.ExpressApp.NCarousel.Web.Resources.noimage.jpg");
                webResourceUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "") + webResourceUrl;
                return new Uri(webResourceUrl);
            }
            return null;
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

        protected override void OnInit(EventArgs e) {
            ClientScriptProxy.Current.Page =
                (Page) ((WebWindowTemplateHttpHandler) HttpContext.Current.Handler).ActualHandler;
            base.OnInit(e);

            string id = HttpContext.Current.Request.QueryString["imageid"];
            if (id != null) {
                IPictureItem item = FindItemByID(id);
                if (item != null && item.Image != null) {
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
    }
}