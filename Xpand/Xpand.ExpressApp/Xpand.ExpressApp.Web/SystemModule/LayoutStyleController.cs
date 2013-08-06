using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Layout;
using Xpand.ExpressApp.Web.Layout;
using Xpand.Utils.Helpers;
using System.Linq;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface ILayoutStyle:IModelNode {
        IModelLayoutStyle Container { get;  }
        IModelLayoutStyle Control { get;  }
        IModelLayoutStyle Caption { get; }
    }

    public interface IModelLayoutViewItemStyle {
        ILayoutStyle LayoutStyle { get; }
    }

    public interface IModelLayoutStyle : IModelNode {
        [Category("eXpand.Layout")]
        FontStyle FontStyle { get; set; }
        [Category("eXpand.Layout")]
        Color FontColor { get; set; }
        [Category("eXpand.Layout")]
        Color BackColor { get; set; }
        [Category("eXpand.Layout")]
        string CssClass { get; set; }
        [Category("eXpand.Layout")]
        string Style { get; set; }
    }

    public class LayoutStyleController:ViewController<DetailView>,IModelExtender {
        LayoutStyleProvider _layoutStyleProvider ;
        protected override void OnActivated() {
            base.OnActivated();
            _layoutStyleProvider = new LayoutStyleProvider();
            View.LayoutManager.LayoutCreated += LayoutManager_LayoutCreated;
            ((IWebLayoutManager)View.LayoutManager).Instantiated += OnInstantiated;
            foreach (var item in View.GetItems<WebPropertyEditor>()) {
                item.ControlCreated+=ItemOnControlCreated;
            }
        }

        void LayoutManager_LayoutCreated(object sender, EventArgs e) {
            View.LayoutManager.LayoutCreated-=LayoutManager_LayoutCreated;
            var modelLayoutStyle = ((IModelLayoutStyle)View.Model);
            if (modelLayoutStyle != null) {
                _layoutStyleProvider.ApplyStyle(modelLayoutStyle, (WebControl) View.LayoutManager.Container);
            }
        }

        void ItemOnControlCreated(object sender, EventArgs eventArgs) {
            var webPropertyEditor = ((WebPropertyEditor) sender);
            webPropertyEditor.ControlCreated-=ItemOnControlCreated;
            var modelLayoutViewItem =View.Model.Layout.ViewItems(webPropertyEditor.Model).Cast<IModelLayoutViewItemStyle>().First();
            _layoutStyleProvider.ApplyLayoutControlStyle(ContainerCell(webPropertyEditor), modelLayoutViewItem.LayoutStyle); 
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ((IWebLayoutManager)View.LayoutManager).Instantiated -= OnInstantiated;
        }

        TableCell ContainerCell(WebPropertyEditor item) {
            var tableEx = ((TableEx)item.Control);
            return ((TableRow)tableEx.Controls[0]).Cells[1];
        }

        void OnInstantiated(object sender, TemplateInstantiatedEventArgs templateInstantiatedEventArgs) {
            var layoutItemTemplateContainer = templateInstantiatedEventArgs.Container as LayoutItemTemplateContainer;
            if (layoutItemTemplateContainer!=null) {
                _layoutStyleProvider.ApplyCaptionControlStyle(layoutItemTemplateContainer);
                _layoutStyleProvider.ApplyContainerControlStyle(layoutItemTemplateContainer);
            }
            var layoutGroupTemplateContainer = templateInstantiatedEventArgs.Container as LayoutGroupTemplateContainer;
            if (layoutGroupTemplateContainer != null) {
                _layoutStyleProvider.ApplyGroupControlStyle(layoutGroupTemplateContainer);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelLayoutViewItem, IModelLayoutViewItemStyle>();
            extenders.Add<IModelDetailView, IModelLayoutStyle>();
            extenders.Add<IModelLayoutGroup, IModelLayoutStyle>();
        }
    }

    public class LayoutStyleProvider {
        static readonly string _backColorPropertyName;
        static readonly string _fontColorPropertyName;
        static readonly string _fontStylePropertyName;
        static readonly string _stylePropertyName;
        static readonly string _cssClassPropertyName;

        static LayoutStyleProvider() {
            _backColorPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.BackColor);
            _fontColorPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.FontColor);
            _fontStylePropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.FontStyle);
            _stylePropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.Style);
            _cssClassPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.CssClass);
        }

        public void ApplyContainerControlStyle(LayoutItemTemplateContainer layoutItemTemplateContainer) {
            var containerControl = layoutItemTemplateContainer.Controls.OfType<Panel>().FirstOrDefault();
            if (containerControl != null && layoutItemTemplateContainer.LayoutItemControl != containerControl) {
                var layoutStyle = ((IModelLayoutViewItemStyle)layoutItemTemplateContainer.Model).LayoutStyle;
                ApplyStyle(layoutStyle.Container, containerControl);
            }
        }

        public void ApplyCaptionControlStyle(LayoutItemTemplateContainer layoutItemTemplateContainer) {
            var layoutStyle = ((IModelLayoutViewItemStyle)layoutItemTemplateContainer.Model).LayoutStyle;
            if (layoutItemTemplateContainer.CaptionControl != null)
                ApplyStyle(layoutStyle.Caption, layoutItemTemplateContainer.CaptionControl);
        }

        public void ApplyLayoutControlStyle(WebControl webControl, ILayoutStyle layoutStyle) {
            if (webControl != null)
                ApplyStyle(layoutStyle.Control, webControl);
        }

        T GetValue<T>(IModelLayoutStyle modelLayoutStyle, string name) {
            var value = modelLayoutStyle.GetValue<T>(name);
            return !IsDefault(value) ? value : default(T);
        }

        bool IsDefault<T>(T value) {
            return Equals(default(T), value);
        }

        public void ApplyStyle(IModelLayoutStyle layoutStyle, WebControl webControl) {
            ApplyColors(layoutStyle, webControl);

            var fontStyle = GetValue<FontStyle>(layoutStyle, _fontStylePropertyName);
            if (fontStyle != FontStyle.Regular)
                webControl.Style.Add(HtmlTextWriterStyle.FontStyle, fontStyle.ToString());

            ApplyCssClassAndInlineStyle(layoutStyle, webControl);
        }

        void ApplyCssClassAndInlineStyle(IModelLayoutStyle layoutStyle, WebControl webControl) {
            var value = GetValue<string>(layoutStyle, _cssClassPropertyName);
            if (!string.IsNullOrEmpty(value))
                webControl.CssClass = value;

            value = GetValue<string>(layoutStyle, _stylePropertyName);
            if (!string.IsNullOrEmpty(value))
                webControl.Style.Value += ";" + value;
        }

        void ApplyColors(IModelLayoutStyle layoutStyle, WebControl webControl) {
            var color = GetValue<Color>(layoutStyle, _backColorPropertyName);
            if (color != Color.Empty) {
                webControl.Style.Add(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(color));
            }

            color = GetValue<Color>(layoutStyle, _fontColorPropertyName);
            if (color != Color.Empty) {
                webControl.Style.Add(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(color));
            }
        }

        public void ApplyGroupControlStyle(LayoutGroupTemplateContainer layoutGroupTemplateContainer) {
            var layoutStyle = ((IModelLayoutStyle)layoutGroupTemplateContainer.Model);
            var containerControl = layoutGroupTemplateContainer.Controls.OfType<WebControl>().FirstOrDefault();
            if (containerControl != null) {
                var groupTemplateContainer = LayoutGroupTemplateContainer(containerControl, 0);
                if (groupTemplateContainer!=null) {
                    ApplyStyle((IModelLayoutStyle)groupTemplateContainer.Model,(WebControl)groupTemplateContainer.Parent);
                    groupTemplateContainer = LayoutGroupTemplateContainer(containerControl, 1);
                    ApplyStyle((IModelLayoutStyle)groupTemplateContainer.Model, (WebControl)groupTemplateContainer.Parent);
                }
                
                ApplyStyle(layoutStyle, containerControl);
            }
        }

        LayoutGroupTemplateContainer LayoutGroupTemplateContainer(WebControl containerControl, int cellIndex) {
            var tableRow = containerControl.Controls[0] as TableRow;
            return tableRow != null ? tableRow.Cells[cellIndex].Controls[0] as LayoutGroupTemplateContainer : null;
        }
    }

}