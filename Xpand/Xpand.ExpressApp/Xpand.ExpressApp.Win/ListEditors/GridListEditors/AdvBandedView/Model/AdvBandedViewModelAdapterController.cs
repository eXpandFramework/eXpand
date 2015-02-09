using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.XtraGrid.Views.BandedGrid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model {
    public class AdvBandedViewModelAdapterController : GridViewModelAdapterControllerBase {
        public AdvBandedViewModelAdapterController() {
            DefaultValues.Add("AutoFillDown", true);
            DefaultValues.Add("ColVIndex", 0);
            DefaultValues.Add("RowIndex", 0);
            NonNullableObjects.Add("RowIndex");
            NonNullableObjects.Add("ColVIndex");
        }

        protected override void ExtendInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsAdvBandedView>();
            extenders.Add<IModelColumn, IModelColumnOptionsAdvBandedView>();

            var builder = new InterfaceBuilder(extenders);

            var columnViewType = typeof(AdvBandedGridView);
            var interfaceBuilderDatas = GetInterfaceBuilderData(columnViewType, typeof(BandedGridColumn)).ToList();
            interfaceBuilderDatas.Add(GetData(typeof(GridBand), typeof(GridBand)));
            var assembly = builder.Build(interfaceBuilderDatas, GetPath(columnViewType.Name));

            builder.ExtendInteface<IModelOptionsAdvBandedView, AdvBandedGridView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnAdvBandedView, BandedGridColumn>(assembly);
            builder.ExtendInteface<IModelGridBand, GridBand>(assembly);
            ExtendWithFont(extenders, builder, assembly);
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
