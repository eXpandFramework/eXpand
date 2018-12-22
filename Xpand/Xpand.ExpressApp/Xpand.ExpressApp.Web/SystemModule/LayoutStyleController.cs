using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Utils;
using Xpand.ExpressApp.Web.Layout;
using Xpand.Utils.Helpers;
using System.Linq;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface ILayoutStyle : IModelNode {
        IModelLayoutStyle ContainerPanel { get; }
        IModelLayoutStyle ContainerCell { get; }
        IModelLayoutStyle Control { get; }
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

    public class LayoutStyleController : ViewController<ObjectView>, IModelExtender {
        LayoutStyleProvider _layoutStyleProvider;
        protected override void OnActivated() {
            base.OnActivated();
            _layoutStyleProvider = new LayoutStyleProvider();
            View.LayoutManager.LayoutCreated += LayoutManager_LayoutCreated;
            if (View.LayoutManager is IWebLayoutManager layoutManager) { layoutManager.Instantiated += OnInstantiated; }
            foreach (var item in View.GetItems<WebPropertyEditor>()) {
                item.ControlCreated += ItemOnControlCreated;
            }
            var listView = View as ListView;
            if (listView?.Editor is ASPxGridListEditor asPxGridListEditor) {
                asPxGridListEditor.CustomCreateCellControl += AsPxGridListEditorOnCustomCreateCellControl;
            }
        }

        void AsPxGridListEditorOnCustomCreateCellControl(object sender, CustomCreateCellControlEventArgs customCreateCellControlEventArgs) {
            ApplyStyle(customCreateCellControlEventArgs.PropertyEditor);
        }

        void LayoutManager_LayoutCreated(object sender, EventArgs e) {
            View.LayoutManager.LayoutCreated -= LayoutManager_LayoutCreated;
            var modelLayoutStyle = ((IModelLayoutStyle)View.Model);
            if (modelLayoutStyle != null) {
                _layoutStyleProvider.ApplyStyle(modelLayoutStyle, (WebControl)View.LayoutManager.Container);
            }
        }

        void ItemOnControlCreated(object sender, EventArgs eventArgs) {
            var webPropertyEditor = ((WebPropertyEditor)sender);
            webPropertyEditor.ControlCreated -= ItemOnControlCreated;
            ApplyStyle(webPropertyEditor);
        }

        void ApplyStyle(WebPropertyEditor webPropertyEditor) {
            var modelLayoutViewItem = ModelLayoutViewItem(webPropertyEditor);
            if (modelLayoutViewItem != null) {
                var containerCell = ContainerCell(webPropertyEditor);
                if (containerCell != null) {
                    _layoutStyleProvider.ApplyContainerCellStyle(containerCell, modelLayoutViewItem.LayoutStyle);
                    _layoutStyleProvider.ApplyControlStyle((WebControl)containerCell.Controls[0], modelLayoutViewItem.LayoutStyle);
                }
            }
        }

        IModelLayoutViewItemStyle ModelLayoutViewItem(WebPropertyEditor webPropertyEditor) {
            Guard.ArgumentNotNull(webPropertyEditor, "webPropertyEditor");

            if (View == null) return (IModelLayoutViewItemStyle)webPropertyEditor.Model;

            var modelDetailView = View.Model as IModelDetailView;
            return modelDetailView == null ? (IModelLayoutViewItemStyle)webPropertyEditor.Model
                       : modelDetailView.Layout.ViewItems(webPropertyEditor.Model).Cast<IModelLayoutViewItemStyle>().FirstOrDefault();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (View.LayoutManager is IWebLayoutManager layoutManager)
                layoutManager.Instantiated -= OnInstantiated;
        }

        TableCell ContainerCell(WebPropertyEditor item) {
            var tableEx = (item.Control as TableEx);
            return ((TableRow) tableEx?.Controls[0])?.Cells[1];
        }

        void OnInstantiated(object sender, TemplateInstantiatedEventArgs templateInstantiatedEventArgs) {

            if (templateInstantiatedEventArgs.Container is LayoutItemTemplateContainer layoutItemTemplateContainer) {
                _layoutStyleProvider.ApplyCaptionControlStyle(layoutItemTemplateContainer);
                _layoutStyleProvider.ApplyContainerControlStyle(layoutItemTemplateContainer);
            }

            if (templateInstantiatedEventArgs.Container is LayoutGroupTemplateContainer layoutGroupTemplateContainer) {
                _layoutStyleProvider.ApplyGroupControlStyle(layoutGroupTemplateContainer);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelLayoutViewItem, IModelLayoutViewItemStyle>();
            extenders.Add<IModelColumn, IModelLayoutViewItemStyle>();
            extenders.Add<IModelDetailView, IModelLayoutStyle>();
            extenders.Add<IModelLayoutGroup, IModelLayoutStyle>();
        }
    }

    public class LayoutStyleProvider {
        static readonly string BackColorPropertyName;
        static readonly string FontColorPropertyName;
        static readonly string FontStylePropertyName;
        static readonly string StylePropertyName;
        static readonly string CssClassPropertyName;

        static LayoutStyleProvider() {
            BackColorPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.BackColor);
            FontColorPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.FontColor);
            FontStylePropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.FontStyle);
            StylePropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.Style);
            CssClassPropertyName = ReflectionExtensions.GetPropertyName<IModelLayoutStyle>(style => style.CssClass);
        }

        public void ApplyContainerControlStyle(LayoutItemTemplateContainer layoutItemTemplateContainer) {
            var containerControl = layoutItemTemplateContainer.Controls.OfType<Panel>().FirstOrDefault();
#pragma warning disable 618
            if (containerControl != null && layoutItemTemplateContainer.LayoutItemControl != containerControl) {
#pragma warning restore 618
                var layoutStyle = ((IModelLayoutViewItemStyle)layoutItemTemplateContainer.Model).LayoutStyle;
                if (layoutStyle != null)
                    ApplyStyle(layoutStyle.ContainerPanel, containerControl);
            }
        }

        public void ApplyCaptionControlStyle(LayoutItemTemplateContainer layoutItemTemplateContainer) {
            var layoutStyle = ((IModelLayoutViewItemStyle)layoutItemTemplateContainer.Model).LayoutStyle;
            if (layoutItemTemplateContainer.CaptionControl != null && layoutStyle != null)
                ApplyStyle(layoutStyle.Caption, layoutItemTemplateContainer.CaptionControl);
        }

        public void ApplyContainerCellStyle(WebControl webControl, ILayoutStyle layoutStyle) {
            if (webControl != null && layoutStyle != null)
                ApplyStyle(layoutStyle.ContainerCell, webControl);
        }

        T GetValue<T>(IModelLayoutStyle modelLayoutStyle, string name) {
            var value = modelLayoutStyle.GetValue<T>(name);
            return !IsDefault(value) ? value : default;
        }

        bool IsDefault<T>(T value) {
            return Equals(default(T), value);
        }

        public void ApplyStyle(IModelLayoutStyle layoutStyle, WebControl webControl) {
            ApplyColors(layoutStyle, webControl);

            var fontStyle = GetValue<FontStyle>(layoutStyle, FontStylePropertyName);
            if (fontStyle != FontStyle.Regular)
                webControl.Style.Add(HtmlTextWriterStyle.FontStyle, fontStyle.ToString());

            ApplyCssClassAndInlineStyle(layoutStyle, webControl);
        }

        void ApplyCssClassAndInlineStyle(IModelLayoutStyle layoutStyle, WebControl webControl) {
            var value = GetValue<string>(layoutStyle, CssClassPropertyName);
            if (!string.IsNullOrEmpty(value))
                webControl.CssClass = value;

            value = GetValue<string>(layoutStyle, StylePropertyName);
            if (!string.IsNullOrEmpty(value))
                webControl.Style.Value += ";" + value;
        }

        void ApplyColors(IModelLayoutStyle layoutStyle, WebControl webControl) {
            var color = GetValue<Color>(layoutStyle, BackColorPropertyName);
            if (color != Color.Empty) {
                webControl.Style.Add(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(color));
            }

            color = GetValue<Color>(layoutStyle, FontColorPropertyName);
            if (color != Color.Empty) {
                webControl.Style.Add(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(color));
            }
        }

        public void ApplyGroupControlStyle(LayoutGroupTemplateContainer layoutGroupTemplateContainer) {
            var layoutStyle = ((IModelLayoutStyle)layoutGroupTemplateContainer.Model);
            var containerControl = layoutGroupTemplateContainer.Controls.OfType<WebControl>().FirstOrDefault();
            if (containerControl != null) {
                var groupTemplateContainer = LayoutGroupTemplateContainer(containerControl, 0);
                if (groupTemplateContainer != null) {
                    ApplyStyle((IModelLayoutStyle)groupTemplateContainer.Model, (WebControl)groupTemplateContainer.Parent);
                    groupTemplateContainer = LayoutGroupTemplateContainer(containerControl, 1);
                    if (groupTemplateContainer != null)
                        ApplyStyle((IModelLayoutStyle)groupTemplateContainer.Model, (WebControl)groupTemplateContainer.Parent);
                }

                ApplyStyle(layoutStyle, containerControl);
            }
        }

        LayoutGroupTemplateContainer LayoutGroupTemplateContainer(WebControl containerControl, int cellIndex) {
            return containerControl.Controls[0] is TableRow tableRow && tableRow.Cells.Cast<TableCell>().Any()&& tableRow.Cells[cellIndex].Controls.Cast<object>().Any() ? tableRow.Cells[cellIndex].Controls[0] as LayoutGroupTemplateContainer : null;
        }

        public void ApplyControlStyle(WebControl webControl, ILayoutStyle layoutStyle) {
            if (webControl != null && layoutStyle != null)
                ApplyStyle(layoutStyle.Control, webControl);
        }
    }

}