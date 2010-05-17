using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eXpand.Persistent.Base.General;
using eXpand.Utils.Web;
using Image = System.Drawing.Image;

namespace eXpand.ExpressApp.Thumbnail.Web {
    public class ThumbnailControl : Panel, INamingContainer {
        IList dataSource;

        public IList DataSource {
            get { return dataSource; }
            set { dataSource = value; }
        }



        IThumbNailItem FindItemByID(string id)
        {
            if (dataSource == null) return null;

            return dataSource.Cast<IThumbNailItem>().FirstOrDefault(item => item.ID == id);
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
            base.OnInit(e);
            Width = Unit.Percentage(100);
            if (HttpContext.Current.Request.QueryString["loadimage"] != null) {
                IThumbNailItem item = FindItemByID(HttpContext.Current.Request.QueryString["loadimage"]);
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

        protected override void CreateChildControls() {
            base.CreateChildControls();
            Refresh();
        }

        public void Refresh() {
            
            Controls.Clear();
            if (Page != null) {
                int i = 0;
                string noImageUrl = ClientScriptProxy.Current.GetWebResourceUrl(this, GetType(), "Thumbnail.Web.Resources.noimage.jpg");
                foreach (IThumbNailItem item in dataSource)
                {
                    Table table = CreateTable();
                    var img = new System.Web.UI.WebControls.Image { ID = ID + "_" + (i++) };
                    var requestTextPictureItemEventArgs = new RequestTextThumbnailItemEventArgs(item);
                    OnRequestText(requestTextPictureItemEventArgs);
                    SetImageProperties(item, img, noImageUrl,requestTextPictureItemEventArgs.Text);
                    CreateImageRow(img, table);
                    CreateTextRow(item, requestTextPictureItemEventArgs, table);
                }
            }
        }

        void CreateTextRow(IThumbNailItem item, RequestTextThumbnailItemEventArgs requestTextThumbnailItemEventArgs, Table table)
        {
            TableCell cell = CreateTextCell(item, requestTextThumbnailItemEventArgs);
            table.Rows.Add(new TableRow());
            table.Rows[1].Cells.Add(cell);
        }

        void CreateImageRow(System.Web.UI.WebControls.Image img, Table table) {
            TableCell cell = CreateImageCell(img);
            table.Rows.Add(new TableRow());
            table.Rows[0].Cells.Add(cell);
        }

        Table CreateTable() {
            var table = new Table();
            table.Style["display"] = "inline";
            Controls.Add(table);
            table.BorderWidth = 0;
            table.CellPadding = 5;
            table.CellSpacing = 0;
            return table;
        }

        TableCell CreateImageCell(System.Web.UI.WebControls.Image img) {
            var cell = new TableCell();
            cell.Controls.Add(img);
            cell.Style["text-align"] = "center";
            return cell;
        }

        TableCell CreateTextCell(IThumbNailItem item, RequestTextThumbnailItemEventArgs requestTextThumbnailItemEventArgs)
        {
            var cell = new TableCell();
            cell.Style["font-size"] = "80%";
            cell.Style["text-align"] = "center";
            cell.Style["word-wrap"] = "break-word";
            cell.Style["word-break"] = "break-word";
            string text1 = string.Format("<a href='{0}&{3}={1}'>{2}</a>",
                                         HttpContext.Current.Request.Url.AbsoluteUri, item.ID, requestTextThumbnailItemEventArgs.Text, ThumbnailListEditor.SelectedId);            
            var text = new Literal { Text = text1 };
            cell.Controls.Add(text);
            return cell;
        }

        void SetImageProperties(IThumbNailItem item, System.Web.UI.WebControls.Image img, string noImageUrl, string text)
        {
            img.AlternateText = text;
            if (item.Image != null) {
                img.ImageUrl = HttpContext.Current.Request.Url.AbsoluteUri + "&loadimage=" + item.ID;
            }
            else if (!(string.IsNullOrEmpty(item.ImagePath))) {
                img.ImageUrl = item.ImagePath;
            }
            else {
                img.ImageUrl = noImageUrl;
            }
        }


        public event EventHandler<RequestTextThumbnailItemEventArgs> RequestText;

        public void OnRequestText(RequestTextThumbnailItemEventArgs e)
        {
            EventHandler<RequestTextThumbnailItemEventArgs> handler = RequestText;
            if (handler != null) handler(this, e);
        }
    }
}