using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridViewModelAdapterController : ModelAdapterController, IModelExtender {
        protected ASPxGridListEditor ASPxGridListEditor;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (ASPxGridListEditor != null)
                ASPxGridListEditor.CreateCustomModelSynchronizer -= GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs e) {
            CustomModelSynchronizerHelper.Assign(e, new GridViewListEditorModelSynchronizer(ASPxGridListEditor));
        }

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && listView.Editor is ASPxGridListEditor) {
                ASPxGridListEditor = (ASPxGridListEditor)listView.Editor;
                ASPxGridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelColumn, IModelColumnOptionsGridViewBand>();
            extenders.Add<IModelOptionsGridView, IModelGridViewOptionsBands>();
            extenders.Add<IModelListView, IModelListViewOptionsGridView>();
            extenders.Add<IModelColumn, IModelColumnOptionsGridView>();

            var builder = new InterfaceBuilder(extenders);

            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(ASPxGridView).Name));

            builder.ExtendInteface<IModelOptionsGridView, ASPxGridView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnGridView, GridViewColumn>(assembly);
            builder.ExtendInteface<IModelGridViewBand, GridViewBandColumn>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(ASPxGridView)) {
                Act =info =>info.Name!="Item"&&(info.DXFilter(BaseGridViewControlTypes(), typeof (object)) ||
                     typeof (PropertiesBase).IsAssignableFrom(info.PropertyType))
            };
            yield return new InterfaceBuilderData(typeof(GridViewColumn)) {
                Act = info => (info.DXFilter(BaseGridViewControlTypes(), typeof(object))) && info.Name != "Width"
            };
            yield return new InterfaceBuilderData(typeof(GridViewBandColumn)) {
                Act = info => {
                    var propertyName = GetPropertyName<GridViewBandColumn>(x=>x.Name);
                    if (info.Name==propertyName)
                        info.AddAttribute(new RequiredAttribute());
                    return info.DXFilter(BaseGridViewBandColumnControlTypes(), typeof(object));
                } 
            };
        }

        IList<Type> BaseGridViewControlTypes() {
            return BaseGridViewBandColumnControlTypes();
        }

        IList<Type> BaseGridViewBandColumnControlTypes() {
            return new List<Type> { typeof(Style) };
        }
    }
}
