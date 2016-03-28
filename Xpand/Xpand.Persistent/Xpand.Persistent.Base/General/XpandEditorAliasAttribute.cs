using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.Persistent.Base.General{
    public class XpandEditorAliasModelUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        private static readonly MemberEditorInfoCalculator _calculator = new MemberEditorInfoCalculator();
        public override void UpdateNode(ModelNode node) {
            var modelViews = ((IModelViews)node);
            foreach (var modelView in modelViews.OfType<IModelObjectView>()) {
                var modelDetailView = modelView as IModelDetailView;
                if (modelDetailView != null)
                    AssignPropertyEditorTypes(modelDetailView.Items.OfType<IModelMemberViewItem>(), ViewType.DetailView);
                else {
                    var modelListView = modelView as IModelListView;
                    if (modelListView != null) AssignPropertyEditorTypes(modelListView.Columns, ViewType.ListView);
                }
            }
        }

        private void AssignPropertyEditorTypes(IEnumerable<IModelMemberViewItem> modelViewItems, ViewType viewType) {
            var items = modelViewItems
                    .Select(item => new { item, Attribute = item.ModelMember.MemberInfo.FindAttribute<XpandEditorAliasAttribute>() }).Where(item => item.Attribute!=null&&item.Attribute.ViewType == viewType);
            foreach (var item in items) {
                item.item.PropertyEditorType = _calculator.GetEditorType(item.item.ModelMember, item.Attribute.Alias);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class XpandEditorAliasAttribute:Attribute{
        readonly string _alias;
        private readonly ViewType _viewType;

        public XpandEditorAliasAttribute(string @alias,ViewType viewType=ViewType.DetailView){
            _alias = alias;
            _viewType = viewType;
        }

        public ViewType ViewType{
            get { return _viewType; }
        }

        public string Alias{
            get { return _alias; }
        }
    }
}