using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;

namespace Xpand.Persistent.Base.TreeNode{

    [DomainComponent]
    [DisplayName("Choose Column")]
    public class ColumnChooserList {
        private static IModelColumn[] SortModelColumns(IModelColumn[] columns) {
            return columns.Where(column => column.Index > -1)
                    .OrderBy(column => column.Index)
                    .Concat(columns.Where(column => column?.Index < 0).OrderBy(column => column.Caption))
                    .ToArray();
        }

        public static ColumnChooserList Create(IObjectSpace objectSpace, IModelColumn[] modelColumns,ColumnChooser parentChooser=null){
            var columnChooserList = new ColumnChooserList();
            foreach (var modelColumn in SortModelColumns(modelColumns)) {
                var columnChooser = ColumnChooser.Create(objectSpace, modelColumn, parentChooser);
                columnChooserList.Columns.Add(columnChooser);
            }
            return columnChooserList;
        }

        public BindingList<ColumnChooser> Columns { get; } = new BindingList<ColumnChooser>();
    }

    [DomainComponent]
    [XafDefaultProperty("Name")]
    public class ColumnChooser : ITreeNode{
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key] 
        public string Key { get; set; }

        private ColumnChooser _parent;
        private IModelColumn _modelColumn;

        ITreeNode ITreeNode.Parent => _parent;
        [VisibleInListView(false)]
        public ColumnChooser Parent{
            get { return (ColumnChooser) ((ITreeNode) this).Parent; }
            set { _parent = value; }
        }

        private BindingList<ColumnChooser> Children { get; } = new BindingList<ColumnChooser>();
        public override string ToString(){
            return Key;
        }

        [DisplayName("Name")]
        public string Caption { get; set; }

        string ITreeNode.Name => Caption;

        
        IBindingList ITreeNode.Children => Children;
        
        [Browsable(false)]
        public IModelColumn ModelColumn => _modelColumn;
        [Browsable(false)]
        public string PropertyName { get; private set; }

        public void Update(IModelColumn modelColumn,  ColumnChooser parentColumnChooser = null){
            
        }

        public static ColumnChooser Create(IObjectSpace objectSpace, IModelColumn modelColumn, ColumnChooser parentColumnChooser){
            var columnChooser = objectSpace.CreateObject<ColumnChooser>();
            columnChooser._modelColumn = modelColumn;
            columnChooser.Parent = parentColumnChooser;
            columnChooser.Caption = modelColumn.Caption;
            columnChooser.PropertyName = modelColumn.PropertyName;
            columnChooser.Key = modelColumn.Id;
            if (parentColumnChooser != null) {
                columnChooser.PropertyName = $"{parentColumnChooser.PropertyName}.{columnChooser.PropertyName}";
                columnChooser.Key = $"{parentColumnChooser.Key}.{columnChooser.Key}";
            }
            return columnChooser;
        }
    }
}