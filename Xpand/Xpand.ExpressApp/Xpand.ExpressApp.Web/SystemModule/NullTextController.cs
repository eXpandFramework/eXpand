using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxEditors;

namespace Xpand.ExpressApp.Web.SystemModule {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NullTextAttribute : Attribute {
        public static NullTextAttribute Default = new NullTextAttribute(null);

        public NullTextAttribute(string nullText) {
            NullText = nullText;
        }

        public string NullText { get; set; }
    }

    public interface IModelMemberNullText {
        [Category("eXpand")]
        string NullText { get; set; }
    }

    public interface IModelLayoutNullText {
        [Category("eXpand")]
        string NullText { get; set; }
    }

    [DomainLogic(typeof(IModelMemberNullText))]
    public static class ModelMemberExtendersLogic {
        public static string Get_NullText(IModelMember modelMember) {
            if (modelMember != null && modelMember.MemberInfo != null) {
                var attribute = modelMember.MemberInfo.FindAttribute<NullTextAttribute>();
                if (attribute != null)
                    return attribute.NullText;
            }
            return null;
        }
    }

    public class NullTextController : ViewController<DetailView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            ((WebLayoutManager)View.LayoutManager).ItemCreated += ViewController_ItemCreated;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var webLayoutManager = View.LayoutManager as WebLayoutManager;
            if (webLayoutManager != null)
                webLayoutManager.ItemCreated -= ViewController_ItemCreated;
        }

        void ViewController_ItemCreated(object sender, ItemCreatedEventArgs e) {
            if (e.TemplateContainer is LayoutItemTemplateContainer) {
                string nullText = GetNullText(e.ModelLayoutElement);
                if (!string.IsNullOrEmpty(nullText)) {
                    ViewItem viewItem = (e.TemplateContainer).ViewItem;
                    if (viewItem.Control == null)
                        viewItem.CreateControl();
                    SetNullText(viewItem, nullText);
                }
            }
        }

        string GetNullText(IModelViewLayoutElement modelViewLayoutElement) {
            string nullText = null;
            var modelLayoutViewItem = ((modelViewLayoutElement)) as IModelLayoutViewItem;
            if (modelLayoutViewItem != null && modelLayoutViewItem.ViewItem is IModelPropertyEditor) {
                var modelMember = ((IModelPropertyEditor)modelLayoutViewItem.ViewItem).ModelMember;
                var modelMemberNullText = modelMember as IModelMemberNullText;
                if (modelMemberNullText != null) {
                    nullText = (modelMemberNullText).NullText;
                }
            }
            if (string.IsNullOrEmpty(nullText) && modelViewLayoutElement is IModelLayoutNullText) {
                nullText = ((IModelLayoutNullText)modelViewLayoutElement).NullText;
            }
            return nullText;
        }

        void SetNullText(ViewItem viewItem, string nullText) {
            if (viewItem is ASPxStringPropertyEditor) {
                var propertyEditor = viewItem as ASPxStringPropertyEditor;
                if (propertyEditor.Editor is ASPxTextBox)
                    (propertyEditor.Editor as ASPxTextBox).NullText = nullText;
                else if (propertyEditor.Editor is ASPxMemo)
                    (propertyEditor.Editor as ASPxMemo).NullText = nullText;
            } else if (viewItem is ASPxDateTimePropertyEditor) {
                var propertyEditor = viewItem as ASPxDateTimePropertyEditor;
                if (propertyEditor.Editor != null)
                    propertyEditor.Editor.NullText = nullText;
            }
        }
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelLayoutItem, IModelLayoutNullText>();
            extenders.Add<IModelMember, IModelMemberNullText>();
        }
        #endregion
    }
}