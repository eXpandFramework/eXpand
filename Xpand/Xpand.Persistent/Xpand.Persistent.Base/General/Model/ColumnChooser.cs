using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Model{
    public interface IColumnChooserListView  {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelValueCalculator("((IModelMemberColumnChooserListView) ModelMember)", "ColumnChooserListView")]
        [DataSourceProperty("Application.Views")]
        [DataSourceCriteria("(AsObjectView Is Not Null) And (AsObjectView.ModelClass Is Not Null) And ('@This.ModelMember.ModelClass' = AsObjectView.ModelClass)")]
        [ModelBrowsable(typeof(ColumnChooserListViewVisibilityCalulator))]
        [ModelReadOnly(typeof(ModelColumnChooserReadOnlyCalculator))]
        IModelListView ColumnChooserListView { get; set; }
    }

    public class ColumnChooserListViewVisibilityCalulator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName){
            var modelMember = (node as IModelMember);
            var memberInfo = modelMember != null
                ? (modelMember.MemberInfo ?? ((IModelColumn) node).ModelMember.MemberInfo)
                : ((IModelColumn) node).ModelMember?.MemberInfo;
            if (memberInfo == null)
                return false;
            return node.Application.BOModel.GetClass(memberInfo.MemberTypeInfo.Type) != null;
        }
    }

    public class ModelColumnChooserReadOnlyCalculator : IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName) {
            return !((IModelSources)node.Application).Modules.Any(m => m is ITreeUser);
        }

        public bool IsReadOnly(IModelNode node, IModelNode childNode) {
            return IsReadOnly(node, "");
        }
    }

}