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
            _defaultValues.Add("EnableMasterViewMode", false);
            _defaultValues.Add("MultiSelect", true);
            _defaultValues.Add("ColumnAutoWidth", true);
            _gridViewMappings.Add("ShowFooter", "IsFooterVisible");
            _gridViewMappings.Add("ShowGroupPanel", "IsGroupPanelVisible");
        }
    }

    public abstract class ColumnViewModelAdapterController : ModelAdapterController, IModelExtender {
        protected readonly HashSet<string> _columnPropertiesToExclude = new HashSet<string>{
            "VisibleIndex",
            "Visible",
            "DisplayFormat",
            "FieldName",
            "PropertyName"
        };

        protected static readonly Dictionary<string, string> _columnMappings = new Dictionary<string, string>();

        protected readonly Dictionary<string, object> _defaultValues = new Dictionary<string, object>();
        protected readonly HashSet<string> _nonNullableObjects = new HashSet<string>();
        protected readonly Dictionary<string, string> _gridViewMappings = new Dictionary<string, string>{
            {"Editable", "AllowEdit"}
        };

        protected HashSet<string> _modelColumnProperties = new HashSet<string>();
        protected HashSet<string> _modelListViewProperties = new HashSet<string>();

        void SetDetaultValues(DynamicModelPropertyInfo info) {
            info.SetDefaultValues(_defaultValues);
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
            if (_nonNullableObjects.Contains(info.Name)) {
                info.AddAttribute(new NullValueAttribute(false));
            }
            if (typeof(BaseView).IsAssignableFrom(typeForDynamicProperties)) {
                if (_gridViewMappings.ContainsKey(info.Name))
                    info.AddAttribute(new BrowsableAttribute(false));
//                    info.CreateValueCalculator("((IModelListView)this.Parent.Parent)." + _gridViewMappings[info.Name]);
            } else if (typeof(GridColumn).IsAssignableFrom(typeForDynamicProperties)) {
                if (_columnMappings.ContainsKey(info.Name)) {
                    var expressionPath = "((IModelColumn)this.Parent)." + _columnMappings[info.Name];
                    info.CreateValueCalculator(expressionPath);
                } else if (_columnPropertiesToExclude.Contains(info.Name))
                    return false;
            }
            return true;
        }

        protected virtual void MapPropertiesToModel(Type typeForDynamicProperties, DynamicModelPropertyInfo info) {
            var parentProperties = _modelColumnProperties;
            if (typeof(BaseView).IsAssignableFrom(typeForDynamicProperties))
                parentProperties = _modelListViewProperties;

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
            _modelListViewProperties = new HashSet<string>(GetProperties(extenders, typeof(IModelListView)));
            _modelColumnProperties = new HashSet<string>(GetProperties(extenders, typeof(IModelColumn)));
            ExtendInterfaces(extenders);
        }

    }

}
