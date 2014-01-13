using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model {
    public abstract class GridViewModelAdapterControllerBase : ColumnViewModelAdapterController {
        protected GridViewModelAdapterControllerBase() {
            DefaultValues.Add("EnableMasterViewMode", false);
            DefaultValues.Add("MultiSelect", true);
            DefaultValues.Add("ColumnAutoWidth", true);
            GridViewMappings.Add("ShowFooter", "IsFooterVisible");
            GridViewMappings.Add("ShowGroupPanel", "IsGroupPanelVisible");
        }
    }

    public abstract class ColumnViewModelAdapterController : ModelAdapterController, IModelExtender {
        protected readonly HashSet<string> ColumnPropertiesToExclude = new HashSet<string>{
            "VisibleIndex",
            "Visible",
            "DisplayFormat",
            "FieldName",
            "PropertyName"
        };

        protected static readonly Dictionary<string, string> ColumnMappings = new Dictionary<string, string>();

        protected readonly Dictionary<string, object> DefaultValues = new Dictionary<string, object>();
        protected readonly HashSet<string> NonNullableObjects = new HashSet<string>();
        protected readonly Dictionary<string, string> GridViewMappings = new Dictionary<string, string>{
            {"Editable", "AllowEdit"}
        };

        protected HashSet<string> ModelColumnProperties = new HashSet<string>();
        protected HashSet<string> ModelListViewProperties = new HashSet<string>();

        void SetDetaultValues(DynamicModelPropertyInfo info) {
            info.SetDefaultValues(DefaultValues);
        }

        public virtual InterfaceBuilderData GetData(Type typeForDynamicProperties, Type controlBaseType) {
            return new InterfaceBuilderData(typeForDynamicProperties) {
                Act = info => Filter(typeForDynamicProperties, controlBaseType, info),
            };
        }

        protected virtual bool Filter(Type typeForDynamicProperties, Type controlBaseType, DynamicModelPropertyInfo info) {
            info.RemoveInvalidTypeConverterAttributes("DevExpress.XtraGrid.Design");
            var filter = info.DXFilter();
            return (ModifyPropertyInfo(typeForDynamicProperties, info)) && filter;
        }

        protected virtual bool ModifyPropertyInfo(Type typeForDynamicProperties, DynamicModelPropertyInfo info) {
            SetDetaultValues(info);
            MapPropertiesToModel(typeForDynamicProperties, info);
            if (NonNullableObjects.Contains(info.Name)) {
                info.AddAttribute(new NullValueAttribute(false));
            }
            if (typeof(BaseView).IsAssignableFrom(typeForDynamicProperties)) {
                if (GridViewMappings.ContainsKey(info.Name))
                    info.AddAttribute(new BrowsableAttribute(false));
//                    info.CreateValueCalculator("((IModelListView)this.Parent.Parent)." + _gridViewMappings[info.Name]);
            } else if (typeof(GridColumn).IsAssignableFrom(typeForDynamicProperties)) {
                if (ColumnMappings.ContainsKey(info.Name)) {
                    var expressionPath = "((IModelColumn)this.Parent)." + ColumnMappings[info.Name];
                    info.CreateValueCalculator(expressionPath);
                } else if (ColumnPropertiesToExclude.Contains(info.Name))
                    return false;
            }
            return true;
        }

        protected virtual void MapPropertiesToModel(Type typeForDynamicProperties, DynamicModelPropertyInfo info) {
            var parentProperties = ModelColumnProperties;
            if (typeof(BaseView).IsAssignableFrom(typeForDynamicProperties))
                parentProperties = ModelListViewProperties;

            if (parentProperties.Contains(info.Name)) {
                info.CreateValueCalculator();
            }
        }

        protected virtual IEnumerable<InterfaceBuilderData> GetInterfaceBuilderData(Type columnViewType, Type columnType) {
            yield return GetData(columnViewType, typeof(BaseView));
            yield return GetData(columnType, typeof(GridColumn));
        }

        protected Assembly BuildAssembly(InterfaceBuilder builder, Type columnViewType, Type gridColumnType) {
            var interfaceBuilderDatas = GetInterfaceBuilderData(columnViewType, gridColumnType);
            return builder.Build(interfaceBuilderDatas, GetPath(columnViewType.Name));
        }

        protected abstract void ExtendInterfaces(ModelInterfaceExtenders extenders);
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            ModelListViewProperties = new HashSet<string>(GetProperties(extenders, typeof(IModelListView)));
            ModelColumnProperties = new HashSet<string>(GetProperties(extenders, typeof(IModelColumn)));
            ExtendInterfaces(extenders);
        }

    }

}
