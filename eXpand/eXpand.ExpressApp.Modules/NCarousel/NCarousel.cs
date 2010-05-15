using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eXpand.NCarousel {
    public class NCarousel : WebControl {
        
        public override Unit Width {
            get { return base.Width == Unit.Empty ? Unit.Pixel(585) : base.Width; }
            set { base.Width = value; }
        }

        [DefaultValue(null)]
        public Alignment Alignment
        {
            get {
                object alignment = ViewState["Alignment"];
                if (alignment != null)
                    return (Alignment) alignment;
                return Alignment.Horizontal;
            }
            set {
                ViewState["Alignment"] = value;
                if (Alignment==Alignment.Vertical) {
                    Width = 115;
                    Height = 645;
                    VisibleItemsCount = 4;
                }
            }
        }
        
        
        public override Unit Height {
            get { return base.Height == Unit.Empty ? Unit.Pixel(175) : base.Height; }
            set { base.Height = value; }
        }
        
        public int VisibleItemsCount {
            get {
                object visibleItemsCount = ViewState["VisibleItemsCount"];
                if (visibleItemsCount != null)
                    return (int) visibleItemsCount;
                return 5;
            }
            set { ViewState["VisibleItemsCount"] = value; }
        }

        
        public Unit ButtonPosition {
            get {
                object previousButtonTop = ViewState["ButtonPosition"];
                if (previousButtonTop != null)
                    return (Unit) previousButtonTop;
                return Unit.Pixel(43);
            }
            set { ViewState["ButtonPosition"] = value; }
        }
        
        protected override HtmlTextWriterTag TagKey {
            get { return HtmlTextWriterTag.Ul; }
        }
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            if (string.IsNullOrEmpty(CssClass))
                CssClass = "jcarousel-skin-tango";
            ClientScriptProxy.Current.RegisterCssResource(this, GetType(), "eXpand.NCarousel.Resources.Skins.skin.css");
            ClientScriptProxy.Current.RegisterCssBlock(this, GetType(), ID+"css", GetCssScript());
            ClientScriptProxy.Current.RegisterClientScriptResource(this, GetType(), "eXpand.NCarousel.Resources.jquery-1.4.2.min.js", ScriptRenderModes.HeaderTop);
            ClientScriptProxy.Current.RegisterClientScriptResource(this, GetType(), "eXpand.NCarousel.Resources.jquery.jcarousel.min.js", ScriptRenderModes.HeaderTop);
            ClientScriptProxy.Current.RegisterClientScriptResource(this, GetType(), "eXpand.NCarousel.Resources.NCarousel.js", ScriptRenderModes.HeaderTop);
            ClientScriptProxy.Current.RegisterClientScriptBlock(this, GetType(), ID+"jscript", GetJScript(),true,ScriptRenderModes.Header);
        }

        string GetCssScript() {
            string alignment = Alignment.ToString().ToLower();
            string position=Alignment==Alignment.Horizontal?"top":"left";
            string prev = string.Format("#{2}div .{0} .jcarousel-prev-" +alignment + " {{"+position + ":{1};}}", CssClass, ButtonPosition,ID);
            string next = string.Format("#{2}div .{0} .jcarousel-next-" + alignment + " {{"+position + ":{1};}}", CssClass, ButtonPosition, ID);
            string widthCss = string.Format("#{3}div .{0} .jcarousel-container-" + alignment + " {{width:{1};height:{2};}}", CssClass, Width, Height, ID);
            string heightCss = string.Format("#{3}div .{0} .jcarousel-clip-" + alignment + " {{width:{1};height:{2};}}", CssClass, Width, Height, ID);
            return prev + Environment.NewLine + next + Environment.NewLine + widthCss + Environment.NewLine + heightCss;
        }

        string GetJScript() {
            string script = string.Format(@"mycarousel_itemList[""" +ID+ @"""]=[{0}];", GetItems());
            string vertical = (Alignment == Alignment.Horizontal ? false : true).ToString().ToLower();
            return string.Format(
                    "{0}{3}jQuery(document).ready(function() {{jQuery('#" + ID + "').jcarousel({{vertical:" + vertical +
                    ",visible: {1},size: {2},itemLoadCallback: {{onBeforeAnimation: mycarousel_itemLoadCallback}}}});}});",
                    script, VisibleItemsCount, Items.Count, Environment.NewLine);
        }
        private readonly List<NCarouselItem> _items=new List<NCarouselItem>();
        public List<NCarouselItem> Items
        {
            get { return _items; }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var htmlWriter = new HtmlTextWriter(sw);
            base.Render(htmlWriter);
            string rendered = Regex.Replace(sb.ToString(), "id=\"([^\"]*)\"", "id=\"" +ID+ "\"");
            rendered = @"<div id=""" + ID + @"div"">" + rendered + "</div>";
            writer.Write(rendered);

        }
        string GetItems() {
            return Items.Aggregate<NCarouselItem, string>(null,(current, nCarouselItem) =>
                                                          current + (@"{url:""" + nCarouselItem.Url + @""",alt:""" + nCarouselItem.Alt + @""", text:""" + nCarouselItem.Text + @"""},"));
        }
    }
}