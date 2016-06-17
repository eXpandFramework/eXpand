using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Web;
using Image = System.Drawing.Image;

namespace Xpand.ExpressApp.Thumbnail.Web {
    public class ThumbnailControl : Panel, INamingContainer {
        public IList DataSource { get; set; }


        IPictureItem FindItemByID(string id) {
            if (DataSource == null) return null;

            return DataSource.Cast<IPictureItem>().FirstOrDefault(item => item.ID == id);
        }

        [DefaultValue(false)]
        public bool HideImages {
            get {
                object hideImages = ViewState["HideImages"];
                return hideImages != null && (bool) hideImages;
            }
            set { ViewState["HideImages"] = value; }
        }
        
        public string DisplayStyle {
            get {
                object displayStyle = ViewState["DisplayStyle"];
                return displayStyle != null ? (string) displayStyle : String.Empty;
            }
            set { ViewState["DisplayStyle"] = value; }
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
                IPictureItem item = FindItemByID(HttpContext.Current.Request.QueryString["loadimage"]);
                if (item != null && item.Image != null) {
                    byte[] buffer = ImageToByteArray(item.Image);
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AppendHeader("content-length", buffer.Length.ToString(CultureInfo.InvariantCulture));
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
//                ClientScriptProxy.Current.Page =(Page) ((WebWindowTemplateHttpHandler) HttpContext.Current.Handler).ActualHandler;
                string noImageUrl = ClientScriptProxy.Current.GetWebResourceUrl(GetType(), "Xpand.ExpressApp.Thumbnail.Web.Resources.noimage.jpg");
                if (DataSource != null) {
                    var rootTable = new Table();
                    Controls.Add(rootTable);
                    var tableRow = new TableRow();
                    var tableCell = new TableCell();
                    tableRow.Cells.Add(tableCell);
                    rootTable.Rows.Add(tableRow);
                    foreach (IPictureItem item in DataSource) {
                        Table table = CreateTable(tableCell.Controls);
                        var img = new System.Web.UI.WebControls.Image { ID = ID + "_" + (i++) };
                        var requestTextPictureItemEventArgs = new RequestTextPictureItemEventArgs(item);
                        OnRequestText(requestTextPictureItemEventArgs);
                        SetImageProperties(item, img, noImageUrl,requestTextPictureItemEventArgs.Text);
                        CreateImageRow(img, table);
                        CreateTextRow(item, requestTextPictureItemEventArgs.Text, table);
                    }
                }
            }
        }

        void CreateTextRow(IPictureItem item, string text, Table table) {
            TableCell cell = CreateTextCell(item, text);
            table.Rows.Add(new TableRow());
            table.Rows[1].Cells.Add(cell);
        }

        void CreateImageRow(System.Web.UI.WebControls.Image img, Table table) {
            TableCell cell =HideImages?new TableCell() : CreateImageCell(img);
            table.Rows.Add(new TableRow());
            table.Rows[0].Cells.Add(cell);
        }

        Table CreateTable(ControlCollection controls) {
            var table = new Table();
            table.Style["display"] = DisplayStyle;
            controls.Add(table);
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

        TableCell CreateTextCell(IPictureItem item, string text) {
            var cell = new TableCell();
            cell.Style["font-size"] = "80%";
            cell.Style["text-align"] = "center";
            cell.Style["word-wrap"] = "break-word";
            cell.Style["word-break"] = "break-word";
            var linkButton = new LinkButton {ID ="link" +ID+item.ID,Text = text};
            
            cell.Controls.Add(linkButton);
            linkButton.Click +=(sender, args) => OnClick(new PictureItemEventArgs(item));
            return cell;
        }



        void SetImageProperties(IPictureItem item, System.Web.UI.WebControls.Image img, string noImageUrl, string text) {
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


        public event EventHandler<PictureItemEventArgs> Click;

        public void OnClick(PictureItemEventArgs e) {
            EventHandler<PictureItemEventArgs> handler = Click;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<RequestTextPictureItemEventArgs> RequestText;

        public void OnRequestText(RequestTextPictureItemEventArgs e)
        {
            EventHandler<RequestTextPictureItemEventArgs> handler = RequestText;
            if (handler != null) handler(this, e);
        }
    }
}