using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.TreeNode{

    [DomainComponent]
    [DisplayName("Choose Column")]
    public class ColumnChooserList {
        static IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo memberTypeInfo, IModelApplication application) {
            var modelClass = application.BOModel.GetClass(memberTypeInfo.Type);
            return modelClass.AllMembers.Select(member => member.MemberInfo).Where(CanBeDisplayed);
        }

        public static ColumnChooserList CreateNested(IObjectSpace objectSpace, ColumnChooser parentChooser,  IModelColumns modelColumns) {
            var columnChoosers = new List<ColumnChooser>();
            var memberInfos = GetMemberInfos(parentChooser.TypeInfo, modelColumns.Application);
            foreach (var memberInfo in memberInfos) {
                var columnChooser = (ColumnChooser)objectSpace.CreateObject(parentChooser.GetType());
                ((ITreeNode)parentChooser).Children.Add(columnChooser);
                var modelMember = GetModelMember(memberInfo, modelColumns.Application);
                columnChooser.Update(modelMember.Caption, memberInfo.MemberTypeInfo,modelMember.Id(), parentChooser);
                columnChoosers.Add(columnChooser);
            }
            var columnChooserList = new ColumnChooserList();
            columnChoosers.OrderBy(chooser => chooser.Caption).Each(columnChooserList.Columns.Add);
            return columnChooserList;
        }

        private static IModelMember GetModelMember(IMemberInfo memberInfo, IModelApplication application) {
            return application.BOModel.GetClass(memberInfo.Owner.Type).AllMembers.First(member => member.MemberInfo == memberInfo);
        }

        static bool CanBeDisplayed(IMemberInfo memberInfo) {
            return !memberInfo.IsKey && memberInfo.FindAttributes<BrowsableAttribute>().All(attribute => attribute.Browsable) && !memberInfo.IsList;
        }

        public static ColumnChooserList CreateRoot(IObjectSpace objectSpace, IModelColumns modelColumns) {
            var columnChooserList = new ColumnChooserList();
            var columns = modelColumns.Where(column => CanBeDisplayed(column.ModelMember.MemberInfo)).Where(column => !column.PropertyName.Contains(".")).OrderBy(column => column.Caption);
            foreach (var modelColumn in columns) {
                var columnChooser = objectSpace.CreateObject<ColumnChooser>();
                var modelMember = modelColumn.ModelMember;
                columnChooser.Update(modelColumn.Caption, modelMember.MemberInfo.MemberTypeInfo,modelMember.Id());
                columnChooserList.Columns.Add(columnChooser);
            }
            return columnChooserList;
        }


        public BindingList<ColumnChooser> Columns { get; } = new BindingList<ColumnChooser>();
    }

    [DomainComponent]
    [XafDefaultProperty("Name")]
    public class ColumnChooser : ITreeNode {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public string Key => GetPath();

        private ColumnChooser _parent;
        

        ITreeNode ITreeNode.Parent => _parent;
        [VisibleInListView(false)]
        public ColumnChooser Parent{
            get { return (ColumnChooser) ((ITreeNode) this).Parent; }
            set { _parent = value; }
        }

        private BindingList<ColumnChooser> Children { get; } = new BindingList<ColumnChooser>();
        public override string ToString(){
            return GetPath();
        }

        [DisplayName("Name")]
        public string Caption { get; set; }
        [Browsable(false)]
        public string ModelId { get; set; }

        string ITreeNode.Name => Caption;

        [Browsable(false)]
        public ITypeInfo TypeInfo { get; set; }
        IBindingList ITreeNode.Children => Children;

        public string GetPath(){
            var paths = new List<string>();
            ExploreParent(chooser => paths.Add(chooser.ModelId));
            return string.Join(".", new[] { ModelId }.Concat(paths).Reverse());
        }

        private void ExploreParent(Action<ColumnChooser> action ){
            var clone = Clone().Parent;
            while (clone != null){
                action(clone);
                clone = clone.Parent;
            }
        }

        private ColumnChooser Clone(){
            var currentObjectClone = new ColumnChooser(){Caption = Caption, Parent = Parent,TypeInfo = TypeInfo,ModelId = ModelId};
            Children.Each(chooser => currentObjectClone.Children.Add(currentObjectClone));
            return currentObjectClone;
        }

        public void Update(string caption, ITypeInfo typeInfo,string modelId, ColumnChooser parentChooser=null){
            ModelId=modelId;
            Parent=parentChooser;
            Caption = caption;
            TypeInfo=typeInfo;
        }
    }
}