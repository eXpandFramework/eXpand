using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using System.Linq;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model {
    public class AdvBandedViewModelAdapterController : GridViewModelAdapterControllerBase {
        public AdvBandedViewModelAdapterController() {
            _defaultValues.Add("AutoFillDown", true);
            _defaultValues.Add("ColVIndex", 0);
            _defaultValues.Add("RowIndex", 0);
            _nonNullableObjects.Add("RowIndex");
            _nonNullableObjects.Add("ColVIndex");
        }

        protected override void ExtendInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsAdvBandedView>();
            extenders.Add<IModelColumn, IModelColumnOptionsAdvBandedView>();

            var builder = new InterfaceBuilder(extenders);

            var columnViewType = typeof(AdvBandedGridView);
            var interfaceBuilderDatas = GetInterfaceBuilderData(columnViewType, typeof(AdvBandedGridColumn)).ToList();
            interfaceBuilderDatas.Add(GetData(typeof(GridBand), typeof(DevExpress.XtraGrid.Views.BandedGrid.GridBand)));
            var assembly = builder.Build(interfaceBuilderDatas, GetPath(columnViewType.Name));

            builder.ExtendInteface<IModelOptionsAdvBandedView, AdvBandedGridView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnAdvBandedView, AdvBandedGridColumn>(assembly);
            builder.ExtendInteface<IModelGridBand, GridBand>(assembly);
        }
        protected override bool ModifyPropertyInfo(Type typeForDynamicProperties, DynamicModelPropertyInfo info) {
            if (typeForDynamicProperties == typeof(GridBand)) {
                if (info.Name == "Width")
                    info.AddAttribute(new NullValueAttribute(false));
                return true;
            }
            return base.ModifyPropertyInfo(typeForDynamicProperties, info);
        }

        protected override bool Filter(Type typeForDynamicProperties, Type controlBaseType, DynamicModelPropertyInfo info) {
            info.SetBrowsable(new Dictionary<string, bool> { { "ColVIndex", true }, { "RowIndex", true } });
            var filter = base.Filter(typeForDynamicProperties, controlBaseType, info);
            if (filter) {
                info.SetCategory(new Dictionary<string, string> { { "ColVIndex", "Appearance" } });
            }
            return filter;
        }
    }

}
