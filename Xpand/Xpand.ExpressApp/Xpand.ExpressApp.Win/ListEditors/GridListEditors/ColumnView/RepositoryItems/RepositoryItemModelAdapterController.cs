using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using System.Linq;
using Fasterflect;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {

    public class RepositoryItemModelAdapterController : ModelAdapterController, IModelExtender {
        public static readonly HashSet<Type> ModelInterfaceTypes = new HashSet<Type>{
            typeof (IModelRepositoryItemTextEdit),
            typeof (IModelRepositoryItemButtonEdit),
            typeof (IModelRepositoryItemPopupBase),
            typeof (IModelRepositoryItemPopupBaseAutoSearchEdit),
            typeof (IModelRepositoryItemComboBox),
            typeof (IModelRepositoryItemPopupContainerEdit),
            typeof (IModelRepositoryFieldPicker),
            typeof (IModelRepositoryItemPopupExpressionEdit),
            typeof (IModelRepositoryItemPopupCriteriaEdit),
            typeof (IModelRepositoryItemImageComboBox),
            typeof (IModelRepositoryItemCheckEdit),
            typeof (IModelRepositoryItemDateEdit),
            typeof (IModelRepositoryItemBaseSpinEdit),
            typeof (IModelRepositoryItemSpinEdit),
            typeof (IModelRepositoryItemObjectEdit),
            typeof (IModelRepositoryItemMemoEdit),
            typeof (IModelRepositoryItemProtectedContentTextEdit),
            typeof (IModelRepositoryItemBlobBaseEdit),
            typeof (IModelRepositoryItemRtfEditEx),
            typeof (IModelRepositoryItemHyperLinkEdit),
            typeof (IModelRepositoryItemPictureEdit),
            typeof (IModelRepositoryItemCalcEdit),
            typeof (IModelRepositoryItemCheckedComboBoxEdit),
            typeof (IModelRepositoryItemColorEdit),
            typeof (IModelRepositoryItemColorPickEdit),
            typeof (IModelRepositoryItemFontEdit),
            typeof (IModelRepositoryItemImageEdit),
            typeof (IModelRepositoryItemLookUpEditBase),
            typeof (IModelRepositoryItemLookUpEdit),
            typeof (IModelRepositoryItemMemoExEdit),
            typeof (IModelRepositoryItemMRUEdit),
            typeof (IModelRepositoryItemBaseProgressBar),
            typeof (IModelRepositoryItemMarqueeProgressBar),
            typeof (IModelRepositoryItemProgressBar),
            typeof (IModelRepositoryItemRadioGroup),
            typeof (IModelRepositoryItemTrackBar),
            typeof (IModelRepositoryItemRangeTrackBar),
            typeof (IModelRepositoryItemTimeEdit),
            typeof (IModelRepositoryItemZoomTrackBar),
        };

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailView = View as DetailView;
            if (detailView != null) new RepositoryItemDetailViewSynchronizer(detailView).ApplyModel();
        }

        static readonly HashSet<string> _excludedProperties = new HashSet<string> { "AutomaticColor" };
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRepositoryItem>();

            var builder = new InterfaceBuilder(extenders);
//            XafTypesInfo.Instance.LoadTypes(typeof(RepositoryItem).Assembly);
            var repositoryItemDescendants = RepositoryItemDescendants().ToArray();
            var assembly = builder.Build(CreateBuilderData(repositoryItemDescendants), GetPath("RepositoryItem"));

            XafTypesInfo.Instance.LoadTypes(assembly);

            foreach (var typeInfo in repositoryItemDescendants) {
                string name = typeInfo.Name;
                var type = Type.GetType(typeof(IModelRepositoryItem).Namespace + ".IModel" + name);
                if (type != null) builder.ExtendInteface(type, typeInfo.Type, assembly);
            }

            var calcType = builder.CalcType(typeof(AppearanceObject), assembly);
            extenders.Add(calcType, typeof(IModelAppearanceFont));
        }


        IEnumerable<InterfaceBuilderData> CreateBuilderData(IEnumerable<ITypeInfo> repositoryItemDescendants) {
            return repositoryItemDescendants.Select(CreateInterfaceBuilderData);
        }

        IEnumerable<ITypeInfo> RepositoryItemDescendants(){
            return ModelInterfaceTypes.Select(GetTypeInfo);
        }

        private ITypeInfo GetTypeInfo(Type interfaceType){
            var repositoryItemAttribute = interfaceType.Attributes(typeof(RepositoryItemAttribute)).Cast<RepositoryItemAttribute>().First();
            return XafTypesInfo.Instance.FindTypeInfo(repositoryItemAttribute.Type);
        }

        InterfaceBuilderData CreateInterfaceBuilderData(ITypeInfo typeInfo) {
            return new InterfaceBuilderData(typeInfo.Type) {
                Act = info => Act(info),
                RootBaseInterface = typeof(IModelRepositoryItem),
                IsAbstract = true
            };
        }

        bool Act(DynamicModelPropertyInfo info) {
            var filter = info.DXFilter(Type.EmptyTypes);
            return (!filter || (!(info.PropertyType == typeof(char) | _excludedProperties.Contains(info.Name)))) && filter;
        }
    }
}
