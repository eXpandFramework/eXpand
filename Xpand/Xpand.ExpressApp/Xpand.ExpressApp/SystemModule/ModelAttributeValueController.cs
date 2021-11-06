using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Fasterflect;
using Xpand.Extensions.TypeExtensions;
using Xpand.Extensions.XAF.ModelExtensions;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelPropertyEditorAttributeValue : IModelNodeAttributeValue {
        new IModelNodeAttributeValueItems AttributeValueItems { get; }
    }

    public interface IModelNodeAttributeValue : IModelNode {
        IModelNodeAttributeValueItems AttributeValueItems { get; }
    }

    [ModelNodesGenerator(typeof(ModelNodeAttributeValueItemsNodeGenerator))]
    public interface IModelNodeAttributeValueItems : IModelNode, IModelList<IModelNodeAttributeValueItem> {

    }

    public class ModelNodeAttributeValueItemsNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    [DomainLogic(typeof(IModelNodeAttributeValueItem))]
    public class ModelNodeAttributeValueItemDomainLogic {
        public ITypeInfo Get_TypeInfo(IModelNodeAttributeValueItem item) {
            if (item.Parent.Parent is IModelPropertyEditor modelPropertyEditor) return modelPropertyEditor.ModelMember.ModelClass.TypeInfo;
            var modelListView = item.Parent.Parent as IModelListView;
            return modelListView?.ModelClass.TypeInfo;
        }
    }

    public class CurrentAttributeValue {
        public CurrentAttributeValue(object currentValue) {
            CurrentValue = currentValue;
        }

        public object CurrentValue { get; }
    }
    public interface IModelNodeAttributeValueItem : IModelNode {
        string Attribute { get; set; }
        string Value { get; set; }
        [Browsable(false)]
        CurrentAttributeValue CurrentAttributeValue { get; set; }
        [CriteriaOptions("TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, XpandModuleBase.UITypeEditor)]
        string Criteria { get; set; }
        [Browsable(false)]
        ITypeInfo TypeInfo { get; }
    }

    public class ModelAttributeValueController : ViewController<ObjectView>, IModelExtender {
        private IEnumerable<IModelNodeAttributeValueItem> _models;
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Application.ViewCreating+=ApplicationOnViewCreating;
                Application.DetailViewCreated += ApplicationOnDetailViewCreated;
                Frame.Disposing+=FrameOnDisposing;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing -= FrameOnDisposing;
            Application.DetailViewCreated -= ApplicationOnDetailViewCreated;
            Application.ViewCreating -= ApplicationOnViewCreating;
        }

        private void ApplicationOnDetailViewCreated(object sender, DetailViewCreatedEventArgs viewCreatedEventArgs) {
            viewCreatedEventArgs.View.Disposing+=ViewOnDisposing;
        }

        private void ViewOnDisposing(object sender, CancelEventArgs cancelEventArgs){
            ((DetailView)sender).Disposing-=ViewOnDisposing;
            if (_models != null)
                foreach (var model in _models) {
                    var propertyType = ((ModelNode)model).Parent.Parent.NodeInfo.GetValueInfo(model.Attribute).PropertyType;
                    ModelNodeExtensions.GetParent<IModelPropertyEditor>(model).CallMethod(new[] { propertyType }, "SetValue", model.Attribute, model.CurrentAttributeValue.CurrentValue);
                    model.CurrentAttributeValue = null;
                }
        }


        private void ApplicationOnViewCreating(object sender, ViewCreatingEventArgs e){
            if (Application.Model.Views[e.ViewID] is IModelDetailView detailView){
                var modelNodeAttributeValueItems = detailView.Items.OfType<IModelNodeAttributeValue>().SelectMany(item => item.AttributeValueItems);
                _models = modelNodeAttributeValueItems.Where(item => ValidByCriteria(item.Criteria,((DetailViewCreatingEventArgs) e).Obj)).ToArray();
                foreach (var item in _models) {
                    var modelNode = ((ModelNode)item);
                    var modelPropertyEditor = ModelNodeExtensions.GetParent<IModelPropertyEditor>(modelNode);
                    var propertyType = ((ModelNode)modelPropertyEditor).NodeInfo.GetValueInfo(item.Attribute).PropertyType;
                    item.CurrentAttributeValue = new CurrentAttributeValue(modelNode.Parent.Parent.GetValue(item.Attribute));
                    modelPropertyEditor.SetValue(item.Attribute, propertyType, item.Value.Change(propertyType));
                }
            }
        }

        private bool ValidByCriteria(string criteria, object obj) {
            var descriptor = ObjectSpace.GetEvaluatorContextDescriptor(obj.GetType());
            var criteriaOperator = ObjectSpace.ParseCriteria(criteria);
            var wrapper = new LocalizedCriteriaWrapper(obj.GetType(), criteriaOperator);
            wrapper.UpdateParametersValues(obj);
            return ObjectSpace.GetExpressionEvaluator(descriptor, wrapper.CriteriaOperator).Fit(obj);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorAttributeValue>();
        }
    }
}
