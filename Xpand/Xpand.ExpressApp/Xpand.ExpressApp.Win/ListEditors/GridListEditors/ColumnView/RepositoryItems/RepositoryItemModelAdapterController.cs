using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using System.Linq;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {
    public class RepositoryItemModelAdapterController : ModelAdapterController, IModelExtender {
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
            var repositoryItemDescendants = XafTypesInfo.Instance.FindTypeInfo(typeof(RepositoryItem)).Descendants;
            return repositoryItemDescendants.Where(info => info.OwnMembers.Any(memberInfo => memberInfo.IsPublic));
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
