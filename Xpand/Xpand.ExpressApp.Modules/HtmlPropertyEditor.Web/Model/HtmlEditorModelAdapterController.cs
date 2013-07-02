using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxHtmlEditor;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model {
    public class HtmlEditorModelAdapterController : ModelAdapterController, IModelExtender {
        public HtmlEditorModelAdapterController() {
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailView = (DetailView) View;
            new HtmlEditorModelSynchronizer(detailView).ApplyModel();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPropertyEditor, IModelPropertyHtmlEditor>();

            var builder = new InterfaceBuilder(extenders);
            var interfaceBuilderDatas = CreateBuilderData();
            var assembly = builder.Build(interfaceBuilderDatas, GetPath(typeof(ASPxHtmlEditor).Name));

            builder.ExtendInteface<IModelHtmlEditor, ASPxHtmlEditor>(assembly);
            builder.ExtendInteface<IModelHtmlEditorToolBar, HtmlEditorToolbar>(assembly);
            builder.ExtendInteface<IModelHtmlEditorShortcut, HtmlEditorShortcut>(assembly);
            builder.ExtendInteface<IModelHtmlEditorToolBarItem, HtmlEditorToolbarItem>(assembly);
            builder.ExtendInteface<IModelToolbarCustomDialogButton, ToolbarCustomDialogButton>(assembly);
            builder.ExtendInteface<IModelHtmlEditorCustomDialog, HtmlEditorCustomDialog>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            var interfaceBuilderData = new InterfaceBuilderData(typeof (ASPxHtmlEditor)){
                Act = info => {
                    if (new[]{typeof (ValidationSettings), typeof (DevExpress.Web.ASPxUploadControl.ValidationSettings)}
                            .Any(type => type.IsAssignableFrom(info.PropertyType))) {
                        return true;
                    }
                    info.RemoveInvalidTypeConverterAttributes("ASPxClasses.Design");
                    return info.DXFilter(BaseHtmlEditorControlTypes(), typeof (object));
                }
            };
            interfaceBuilderData.ReferenceTypes.Add(typeof(CursorConverter));
            yield return interfaceBuilderData;
            yield return new InterfaceBuilderData(typeof(HtmlEditorToolbar)){
                Act = info => info.DXFilter()
            };
            yield return new InterfaceBuilderData(typeof(HtmlEditorToolbarItem)){
                Act = info => {
                    var dxFilter = info.DXFilter(new[]{typeof (ToolbarItemImageProperties)}, typeof (object));
                    return dxFilter;
                }
            };
            yield return new InterfaceBuilderData(typeof(HtmlEditorShortcut)){
                Act = info => info.DXFilter()
            };
            yield return new InterfaceBuilderData(typeof(HtmlEditorCustomDialog)){
                Act = info => {
                    if (info.Name == "Name")
                        info.AddAttribute(new BrowsableAttribute(false));
                    return info.DXFilter();
                }
            };
            yield return new InterfaceBuilderData(typeof(ToolbarCustomDialogButton)) {
                Act = info => {
                    if (info.Name == "Name" || info.Name == "CommandName") {
                        info.AddAttribute(new BrowsableAttribute(false));
                        info.AddAttribute(new BrowsableAttribute(false));
                    }
                    return info.DXFilter();
                }
            };
        }

        Type[] BaseHtmlEditorControlTypes() {
            return new[]{
                typeof (ASPxHtmlEditorSettingsBase),typeof(StylesBase),typeof(ImagesBase),typeof (ClientSideEvents)
            };
        }
    }
}
