using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Xpand.NCarousel {
    public class NCarousel : WebControl, INamingContainer {
        public NCarousel() {
            Css = new NCarouselCss();
        }

        [DefaultValue(null)]
        public NCarouselCss Css {
            get {
                object css = ViewState["Css"];
                return css != null ? (NCarouselCss)css : null;
            }
            set { ViewState["Css"] = value; }
        }


        [DefaultValue(false)]
        public bool HideImages {
            get {
                object hideImages = ViewState["HideImages"];
                return hideImages != null && (bool)hideImages;
            }
            set {
                ViewState["HideImages"] = value;
                Css.SetDefaultValues(Alignment, value);
            }
        }

        [DefaultValue(null)]
        public Alignment Alignment {
            get {
                object alignment = ViewState["Alignment"];
                return alignment != null ? (Alignment)alignment : Alignment.Horizontal;
            }
            set {
                ViewState["Alignment"] = value;
                Css.SetDefaultValues(value, HideImages);
            }
        }

        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            if (string.IsNullOrEmpty(CssClass))
                CssClass = Css.ClassName;

            Page.Controls.Add(new LiteralControl("<link href=\"" + ResolveUrl("Xpand.NCarousel.Resources.Skins.skin.css") + "\" rel=\"stylesheet\" type=\"text/css\" >\r\n"));
            Page.Controls.Add(new LiteralControl(@"<style type=""text/css"">" + GetCssScript() + "</style>"));
            Page.ClientScript.RegisterClientScriptResource(typeof(NCarousel), "Xpand.NCarousel.Resources.jquery-1.4.2.min.js");
            Page.ClientScript.RegisterClientScriptResource(typeof(NCarousel), "Xpand.NCarousel.Resources.jquery.jcarousel.min.js");
            Page.ClientScript.RegisterClientScriptResource(typeof(NCarousel), "Xpand.NCarousel.Resources.NCarousel.js");
        }

        protected override void CreateChildControls() {
            base.CreateChildControls();
            Page.ClientScript.RegisterClientScriptBlock(typeof(NCarousel), ClientID + "jscript", GetJScript(), true);
        }

        string GetCssScript() {
            if (Css.AllowOverride)
                return null;
            var container = "#" + ClientID + "div ." + CssClass + " .jcarousel-container{" + Css.Container + "}" + Environment.NewLine;
            var item = "#" + ClientID + "div ." + CssClass + " .jcarousel-item{" + Css.Iτem + "}" + Environment.NewLine;
            var clip = "#" + ClientID + "div ." + CssClass + " .jcarousel-clip{" + Css.Clip + "}" + Environment.NewLine;
            var next = "#" + ID + "div ." + CssClass + " .jcarousel-next{" + Css.Next + "}" + Environment.NewLine;
            var previous = "#" + ID + "div ." + CssClass + " .jcarousel-prev{" + Css.Previous + "}" + Environment.NewLine;

            return container + item + clip + next + previous;
        }

        string GetJScript() {
            string script = string.Format(@"mycarousel_itemList[""" + ClientID + @"""]=[{0}];", GetItems());
            string vertical = (Alignment == Alignment.Horizontal ? false : true).ToString().ToLower();
            return string.Format(
                    "{0}{2}jQuery(document).ready(function() {{jQuery('#" + ClientID + "').jcarousel({{vertical:" + vertical +
                    ",size: {1},itemLoadCallback: {{onBeforeAnimation: mycarousel_itemLoadCallback}}}});}});",
                    script, Items.Count, Environment.NewLine);
        }

        protected override HtmlTextWriterTag TagKey {
            get { return HtmlTextWriterTag.Ul; }
        }
        private readonly List<NCarouselItem> _items = new List<NCarouselItem>();
        public List<NCarouselItem> Items {
            get { return _items; }
        }
        protected override void Render(HtmlTextWriter writer) {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var htmlWriter = new HtmlTextWriter(sw);
            base.Render(htmlWriter);
            string rendered = @"<div id=""" + ClientID + @"div"">" + sb + "</div>";
            writer.Write(rendered);

        }
        string GetItems() {
            return Items.Aggregate<NCarouselItem, string>(null, (current, nCarouselItem) =>
                                                          current + (@"{url:""" + (HideImages ? null : nCarouselItem.Url) + @""",alt:""" + nCarouselItem.Alt + @""", text:""" + nCarouselItem.Text + @"""},"));
        }

    }
}