using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using Fasterflect;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {
    public class RepositoryItemColumnViewSynchronizer : RepositoryItemSynchronizer<DevExpress.XtraGrid.Views.Base.ColumnView, IModelListView> {
        public RepositoryItemColumnViewSynchronizer(DevExpress.XtraGrid.Views.Base.ColumnView control, IModelListView modelNode)
            : base(control, modelNode) {
        }

        protected override void ApplyModelCore() {
            foreach (var modelColumn in Model.Columns.OfType<IModelMemberViewItemRepositoryItem>()) {
                var gridColumn = Control.Columns[modelColumn.PropertyName];
                if (gridColumn != null) {
                    var modelRepositoryItems = GetRepositoryItems(gridColumn.ColumnEdit, modelColumn);
                    foreach (var modelRepositoryItem in modelRepositoryItems) {
                        ApplyModel(modelRepositoryItem, gridColumn.ColumnEdit, ApplyValues);
                    }
                }
            }
        }

        IEnumerable<IModelRepositoryItem> GetRepositoryItems(RepositoryItem repositoryItem, IModelMemberViewItemRepositoryItem modelMemberViewItem) {
            return modelMemberViewItem.RepositoryItems.Where(item => FindRepository(repositoryItem, item));
        }

        public override void SynchronizeModel() {

        }
    }

    public class RepositoryItemDetailViewSynchronizer : RepositoryItemSynchronizer<DetailView, IModelDetailView> {
        public RepositoryItemDetailViewSynchronizer(DetailView control)
            : base(control, control.Model) {
        }

        protected override void ApplyModelCore() {
            foreach (var viewItem in ViewItems()) {
                var dxPropertyEditor = Control.GetItems<DXPropertyEditor>().FirstOrDefault(editor => editor.Model == viewItem);
                if (dxPropertyEditor != null) {
                    var repositoryItem = dxPropertyEditor.Control.Properties;
                    var modelRepositoryItems = GetRepositoryItems(repositoryItem, viewItem);
                    foreach (var modelRepositoryItem in modelRepositoryItems) {
                        ApplyModel(modelRepositoryItem, repositoryItem, ApplyValues);
                    }
                }
            }
        }

        IEnumerable<IModelMemberViewItemRepositoryItem> ViewItems() {
            return Model.Items.OfType<IModelMemberViewItemRepositoryItem>().Where(item => item.RepositoryItems.Any());
        }

        IEnumerable<IModelRepositoryItem> GetRepositoryItems(RepositoryItem repositoryItem, IModelMemberViewItemRepositoryItem modelMemberViewItem) {
            return modelMemberViewItem.RepositoryItems.Where(item => FindRepository(repositoryItem, item));
        }


        public override void SynchronizeModel() {
        }
    }

    public abstract class RepositoryItemSynchronizer<T, TV> : Persistent.Base.ModelAdapter.ModelSynchronizer<T, TV> where TV : IModelObjectView {
        protected RepositoryItemSynchronizer(T component, TV modelNode)
            : base(component, modelNode) {
        }

        protected override PropertyDescriptor GetPropertyDescriptor(PropertyDescriptorCollection properties, ModelValueInfo valueInfo, object component, ModelNode node){
            if (component is AppearanceObject)
                if (typeof(IModelAppearanceFont).Properties().Select(info => info.Name).Contains(valueInfo.Name))
                    return new FontPropertyDescriptor(valueInfo.Name, new Attribute[0], GetApplyModelNodeValue, GetModelValueInfos(node),node);
            return base.GetPropertyDescriptor(properties, valueInfo, component,node);
        }

        class FontPropertyDescriptor:PropertyDescriptor{
            private readonly Func<ModelNode, ModelValueInfo, object> _getApplyModelNodeValue;
            private readonly IEnumerable<ModelValueInfo> _modelValueInfos;
            private readonly ModelNode _modelNode;

            public FontPropertyDescriptor(string name, Attribute[] attrs, Func<ModelNode, ModelValueInfo, object> getApplyModelNodeValue, IEnumerable<ModelValueInfo> modelValueInfos,ModelNode modelNode)
                : base(name, attrs){
                _getApplyModelNodeValue = getApplyModelNodeValue;
                _modelValueInfos = modelValueInfos;
                _modelNode = modelNode;
            }

            public override bool CanResetValue(object component){
                throw new NotImplementedException();
            }

            public override object GetValue(object component){
                var font = ((AppearanceObject) component).Font;
                return Name=="FontName" ? font.Name : font.GetPropertyValue(Name);
            }

            public override void ResetValue(object component){
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value){
                var font = ((AppearanceObject) component).Font;
                var name = GetNodeValue("FontName", font, "Name");
                var size = GetNodeValue("Size", font, "Size");
                var fontStyle = GetFontStyle(font);
                var unit = GetNodeValue("Unit", font, "Unit");
                ((AppearanceObject)component).Font = new Font(name.ToString(), (float)size, fontStyle, (GraphicsUnit)unit);
            }

            private FontStyle GetFontStyle(Font font){
                var bold = (bool) GetNodeValue("Bold", font, "Bold");
                var italic = (bool) GetNodeValue("Italic", font, "Italic");
                var strikeout = (bool) GetNodeValue("Strikeout", font, "Strikeout");
                var underline = (bool) GetNodeValue("Underline", font, "Underline");
                var fontStyle = FontStyle.Regular;
                if (bold){
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

            private object GetNodeValue(string name, Font font, string propertyName){
                var modelValueInfo = _modelValueInfos.First(info => info.Name == name);
                return _getApplyModelNodeValue(_modelNode, modelValueInfo) ??
                                font.GetPropertyValue(propertyName);
            }

            public override bool ShouldSerializeValue(object component){
                throw new NotImplementedException();
            }

            public override Type ComponentType{
                get { throw new NotImplementedException(); }
            }

            public override bool IsReadOnly{
                get { throw new NotImplementedException(); }
            }

            public override Type PropertyType{
                get{return Name=="FontName" ? typeof(string) : typeof(Font).Property(Name).PropertyType;}
            }
        }
        protected bool FindRepository(RepositoryItem repositoryItem, IModelRepositoryItem item) {
            if (!item.NodeEnabled) return false;
            var repoType = repositoryItem.GetType();
            var itemTypeName = item.GetType().Name;
            while ("Model" + repoType.Name != itemTypeName) {
                repoType = repoType.BaseType;
                if (repoType == null)
                    return false;
            }
            return true;
        }

    }
}