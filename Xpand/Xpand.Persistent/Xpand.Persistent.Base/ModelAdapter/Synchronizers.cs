using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.ModelAdapter {
    public class ObjectModelSynchronizer : ModelSynchronizer<object, IModelNode> {
        public ObjectModelSynchronizer(object component, IModelNode modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            var modelModelAdapter = Model as IModelModelAdapter;
            if (modelModelAdapter != null){
                foreach (var modelAdapter in modelModelAdapter.GetContextAdapters()){
                    ApplyModel(modelAdapter,Control,ApplyValues);
                }
                ApplyModel(Model, Control, ApplyValues);
            }
            var modelAdapterLink = Model as IModelModelAdapterLink;
            if (modelAdapterLink != null && modelAdapterLink.ModelAdapter != null)
                ApplyModel(modelAdapterLink.ModelAdapter, Control, ApplyValues);
            else
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {
            throw new NotImplementedException();
        }
    }

    public abstract class ModelSynchronizer<TComponent, TModelNode> : DevExpress.ExpressApp.Model.ModelSynchronizer<TComponent, TModelNode> where TModelNode : IModelNode {
        public static readonly HashSet<string> ExcludedNodeMembers =
            new HashSet<string>(new[] { "Id", "Index", "Removed", "IsNewNode", "IsRemovedNode" });
        protected ModelSynchronizer(TComponent component, TModelNode modelNode)
            : base(component, modelNode) {
        }

        protected void SynchronizeValues(ModelNode modelNode, object component, PropertyDescriptorCollection properties) {

            foreach (var valueInfo in GetModelValueInfos(modelNode)) {
                var propertyDescriptor = properties.Find(valueInfo.Name, false);
                if (propertyDescriptor != null) {
                    var propertyValue = GetPropertyValue(component, propertyDescriptor, valueInfo, modelNode);
                    var modelValue = GetSynchronizeValuesNodeValue(modelNode, valueInfo, propertyDescriptor, valueInfo.PropertyType.IsNullableType(), component);
                    if (modelValue != null && !modelValue.Equals(propertyValue)) {
                        modelNode.SetValue(valueInfo.Name, propertyValue);
                    }
                }
            }
        }

        protected virtual object GetSynchronizeValuesNodeValue(ModelNode modelNode, ModelValueInfo valueInfo, PropertyDescriptor propertyDescriptor, bool isNullableType, object component) {
            return !isNullableType ? GetNodeValueCore(modelNode, valueInfo) : null;
        }

        protected virtual object GetPropertyValue(object component, PropertyDescriptor propertyDescriptor, ModelValueInfo valueInfo, ModelNode modelNode) {
            return propertyDescriptor.GetValue(component);
        }

        protected virtual bool IsDefaultValue(object value, PropertyDescriptor propertyDescriptor) {
            var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().SingleOrDefault();
            return defaultValueAttribute != null && defaultValueAttribute.Value.Equals(value);
        }

        protected void ApplyModel(IModelNode node, object component, Action<ModelNode, object, PropertyDescriptorCollection> action) {
            CheckComponentType(component);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            var mNode = (ModelNode)node;
            action.Invoke(mNode, component, properties);
            for (int i = 0; i < node.NodeCount; i++) {
                var modelNode = mNode[i];
                var propertyDescriptor = properties.Find(modelNode.Id, false);
                if (propertyDescriptor != null) {
                    ApplyModel(modelNode, propertyDescriptor.GetValue(component), action);
                }
            }
        }

        protected virtual void CheckComponentType(object component) {
            if (component is IModelNode)
                throw new ArgumentOutOfRangeException("component");
        }

        protected void ApplyValues(ModelNode node, object component, PropertyDescriptorCollection properties) {
            foreach (var valueInfo in GetModelValueInfos(node)){
                var propertyDescriptor = GetPropertyDescriptor(properties, valueInfo,component,node);
                if (propertyDescriptor != null) {
                    var nodeValue = GetApplyModelNodeValue(node, valueInfo);
                    if (nodeValue != null) {
                        var propertyType = propertyDescriptor.PropertyType;
                        var propertyValue = propertyDescriptor.GetValue(component);
                        if ((!IsDefaultCoreValue(nodeValue, propertyType) || (!nodeValue.Equals(propertyValue))) && propertyType.IsValidEnum(nodeValue)) {
                            if (!nodeValue.Equals(propertyValue))
                                propertyDescriptor.SetValue(component, nodeValue);
                        }
                    }
                }
            }
        }

        protected virtual PropertyDescriptor GetPropertyDescriptor(PropertyDescriptorCollection properties, ModelValueInfo valueInfo, object component, ModelNode node) {
            if (component is AppearanceObject)
                if (typeof(IModelAppearanceFont).Properties().Select(info => info.Name).Contains(valueInfo.Name))
                    return new FontPropertyDescriptor(valueInfo.Name, new Attribute[0], GetApplyModelNodeValue,  node);
            return properties.Find(valueInfo.Name, false);
        }

        protected virtual object GetApplyModelNodeValue(ModelNode node, ModelValueInfo valueInfo) {
            return GetNodeValueCore(node, valueInfo);
        }

        protected object PropertyDefaultValue(object value, ModelNode node, PropertyDescriptor propertyDescriptor, ModelValueInfo valueInfo, object component) {
            var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
            if (defaultValueAttribute == null) {
                if (propertyDescriptor.PropertyType.IsStruct())
                    return propertyDescriptor.PropertyType.CreateInstance();
            } else if (defaultValueAttribute.Value.Equals(propertyDescriptor.GetValue(component))) {
                return null;
            }
            return NodeRealValueCore(valueInfo, node);
        }

        object NodeRealValueCore(ModelValueInfo valueInfo, ModelNode node) {
            var nodeValue = GetNodeValueCore(node, valueInfo);
            if (nodeValue == null && !valueInfo.IsReadOnly && valueInfo.PropertyType != typeof(string)) {
                if (valueInfo.PropertyType.IsStruct())
                    return valueInfo.PropertyType.CreateInstance();
                if (valueInfo.PropertyType.IsNullableType())
                    return valueInfo.PropertyType.GetGenericArguments()[0].CreateInstance();
                throw new NotImplementedException(valueInfo.PropertyType.ToString());
            }
            return nodeValue;
        }

        protected virtual object GetNodeValueCore(ModelNode node, ModelValueInfo valueInfo) {
            if (valueInfo.IsReadOnly)
                return null;
            var nodeValue = node.GetValue(valueInfo.Name);
            return nodeValue != null && (valueInfo.PropertyType.IsStruct() && nodeValue.Equals(valueInfo.PropertyType.CreateInstance()))? null: nodeValue;
        }

        protected virtual IEnumerable<ModelValueInfo> GetModelValueInfos(IModelNode modelNode) {
            return IsDisabled(modelNode) ? Enumerable.Empty<ModelValueInfo>() : ((ModelNode)modelNode).NodeInfo.ValuesInfo.Where(IsNotExcluded);
        }

        bool IsDisabled(IModelNode modelNode) {
            return modelNode is IModelNodeEnabled && !((IModelNodeEnabled)modelNode).NodeEnabled;
        }

        bool IsNotExcluded(ModelValueInfo info) {
            return !ExcludedNodeMembers.Contains(info.Name);
        }

        protected virtual bool IsDefaultCoreValue(object value, Type propertyType) {
            if (value == null)
                return true;
            if (value is string)
                return ReferenceEquals(value, "");
            return propertyType.IsValueType && propertyType.CreateInstance().Equals(value);
        }
    }
    public abstract class ModelListSynchronizer : ModelSynchronizer {
        private readonly ModelSynchronizerList _modelSynchronizerList;

        protected ModelListSynchronizer(object control, IModelNode model)
            : base(control, model) {
            _modelSynchronizerList = new ModelSynchronizerList();

        }

        protected override void ApplyModelCore() {
            _modelSynchronizerList.ApplyModel();
        }
        public override void SynchronizeModel() {
            _modelSynchronizerList.SynchronizeModel();
        }

        public ModelSynchronizerList ModelSynchronizerList {
            get { return _modelSynchronizerList; }
        }

        public override void Dispose() {
            base.Dispose();
            _modelSynchronizerList.Dispose();
        }

    }

    public class FontBuilder {

        private readonly IModelAppearanceFont _modelAppearanceFont;
        private readonly Font _font;

        public FontBuilder(IModelAppearanceFont modelAppearanceFont, Font font) {
            _modelAppearanceFont = modelAppearanceFont;
            _font = font;
        }

        public IModelNode ModelAppearanceFont {
            get { return _modelAppearanceFont; }
        }

        public Font GetFont() {
            var name = GetNodeValue("FontName", "Name");
            var size = GetNodeValue("Size", "Size");
            var fontStyle = GetFontStyle();
            var unit = GetNodeValue("Unit", "Unit");
            return new Font(name.ToString(), (float)size, fontStyle, (GraphicsUnit)unit);
        }

        private FontStyle GetFontStyle() {
            var bold = (bool)GetNodeValue("Bold", "Bold");
            var italic = (bool)GetNodeValue("Italic", "Italic");
            var strikeout = (bool)GetNodeValue("Strikeout", "Strikeout");
            var underline = (bool)GetNodeValue("Underline", "Underline");
            var fontStyle = FontStyle.Regular;
            if (bold) {
                fontStyle = FontStyle.Bold;
            }
            if (italic)
                fontStyle |= FontStyle.Italic;
            if (strikeout)
                fontStyle |= FontStyle.Strikeout;
            if (underline)
                fontStyle |= FontStyle.Underline;
            return fontStyle;
        }

        object GetNodeValue(string name, string propertyName) {
            return GetNodeValueCore(_modelAppearanceFont, name) ?? _font.GetPropertyValue(propertyName);
        }

        protected virtual object GetNodeValueCore(IModelNode modelNode, string name) {
            return modelNode.HasValue(name) ? modelNode.GetValue(name) : null;
        }
    }

    class FontBuilderSynch:FontBuilder{
        private readonly Func<ModelNode, ModelValueInfo, object> _getApplyModelNodeValue;

        public FontBuilderSynch(IModelAppearanceFont modelAppearanceFont, Font font, Func<ModelNode, ModelValueInfo, object> getApplyModelNodeValue) : base(modelAppearanceFont, font){
            _getApplyModelNodeValue = getApplyModelNodeValue;
        }

        protected override object GetNodeValueCore(IModelNode modelNode, string name){
            var node = ((ModelNode) modelNode);
            var modelValueInfo = node.GetValueInfo(name);
            return _getApplyModelNodeValue(node, modelValueInfo);
        }
    }
    public class FontPropertyDescriptor : PropertyDescriptor {
        private readonly Func<ModelNode, ModelValueInfo, object> _getApplyModelNodeValue;
        private readonly ModelNode _modelNode;

        public FontPropertyDescriptor(string name, Attribute[] attrs, Func<ModelNode, ModelValueInfo, object> getApplyModelNodeValue,  ModelNode modelNode)
            : base(name, attrs) {
            _getApplyModelNodeValue = getApplyModelNodeValue;
            _modelNode = modelNode;
        }

        public override bool CanResetValue(object component) {
            throw new NotImplementedException();
        }

        public override object GetValue(object component) {
            var font = ((AppearanceObject)component).Font;
            return Name == "FontName" ? font.Name : font.GetPropertyValue(Name);
        }

        public override void ResetValue(object component) {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value) {
            var appearanceObject = ((AppearanceObject) component);
            Font font = appearanceObject.Font;
            var fontBuilderSynch = new FontBuilderSynch((IModelAppearanceFont) _modelNode, font, _getApplyModelNodeValue);
            Font font1 = fontBuilderSynch.GetFont();
            if (!Equals(font1, font)){
                appearanceObject.BeginUpdate();
                appearanceObject.Options.UseFont = true;
                appearanceObject.Font = font1;
                appearanceObject.EndUpdate();
            }
        }

        public override bool ShouldSerializeValue(object component) {
            throw new NotImplementedException();
        }

        public override Type ComponentType {
            get { throw new NotImplementedException(); }
        }

        public override bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        public override Type PropertyType {
            get { return Name == "FontName" ? typeof(string) : typeof(Font).Property(Name).PropertyType; }
        }
    }

}