using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
using System.Linq;
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
            typeof (IModelRepositoryItemLookupEdit),
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
            XafTypesInfo.Instance.LoadTypes(typeof(RepositoryItem).Assembly);
            var assembly = builder.Build(CreateBuilderData(), GetPath("RepositoryItem"));

            XafTypesInfo.Instance.LoadTypes(assembly);

            foreach (var typeInfo in RepositoryItemDescendants()) {
                string name = typeInfo.Name;
                var type = Type.GetType(typeof(IModelRepositoryItem).Namespace + ".IModel" + name, true);
                builder.ExtendInteface(type, typeInfo.Type, assembly);
            }

        }

        protected string GeneratedEmptyInterfacesCode() {
            return InterfaceBuilder.GeneratedEmptyInterfacesCode(RepositoryItemDescendants(),
                typeof(IModelRepositoryItem), (info, type, arg3) =>
                    InterfaceBuilder.GeneratedDisplayNameCode(arg3.Replace("IModelRepositoryItem", "")));
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            return RepositoryItemDescendants().Select(CreateInterfaceBuilderData);
        }

        IEnumerable<ITypeInfo> RepositoryItemDescendants() {
            var repositoryItemDescendants = ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(RepositoryItem)), false);
            var names = ModelInterfaceTypes.Select(type => type.Name.Substring(6)).ToList();
            return repositoryItemDescendants.Where(info => info.OwnMembers.Any(memberInfo => memberInfo.IsPublic) && names.Contains(info.Type.Name)).ToList();
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
