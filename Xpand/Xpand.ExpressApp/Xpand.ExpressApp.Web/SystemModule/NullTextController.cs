using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule {
    [AttributeUsage(AttributeTargets.Property)]
    public class NullTextAttribute : Attribute {
        public static NullTextAttribute Default = new NullTextAttribute(null);

        public NullTextAttribute(string nullText) {
            NullText = nullText;
        }

        public string NullText { get; set; }
    }

    public interface IModelMemberNullText {
        [Category(AttributeCategoryNameProvider.Xpand)]
        string NullText { get; set; }
    }

    public interface IModelLayoutNullText {
        [Category("eXpand")]
        string NullText { get; set; }
    }

    [DomainLogic(typeof(IModelMemberNullText))]
    public static class ModelMemberExtendersLogic {
        public static string Get_NullText(IModelMember modelMember) {
            var attribute = modelMember?.MemberInfo?.FindAttribute<NullTextAttribute>();
            return attribute?.NullText;
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
            if (modelLayoutViewItem?.ViewItem is IModelPropertyEditor) {
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

        void SetNullText(ViewItem viewItem, string nullText){
            var editor = viewItem as ASPxStringPropertyEditor;
            if (editor != null) {
                var propertyEditor = editor;
                var box = propertyEditor.Editor as ASPxTextBox;
                if (box != null)
                    box.NullText = nullText;
                else if (propertyEditor.Editor is ASPxMemo)
                    ((ASPxMemo) propertyEditor.Editor).NullText = nullText;
            } else{
                var timePropertyEditor = viewItem as ASPxDateTimePropertyEditor;
                var propertyEditor = timePropertyEditor;
                if (propertyEditor?.Editor != null)
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