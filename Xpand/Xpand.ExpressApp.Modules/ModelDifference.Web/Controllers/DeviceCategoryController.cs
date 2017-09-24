using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Web.Controllers {
    public class DeviceCategoryController:ObjectViewController<ObjectView,ModelDifferenceObject>{
        protected override void OnActivated(){
            base.OnActivated();
            var member = GetDeviceCategoryModelMember();
            if (member != null) member.AllowEdit = Application.ApplicationDeviceModelsEnabled();
        }

        private IModelCommonMemberViewItem GetDeviceCategoryModelMember(){
            var modelListView = View.Model as IModelListView;
            var modelColumn = modelListView?.Columns.FirstOrDefault(
                column => column.ModelMember.MemberInfo.Name == nameof(ModelDifferenceObject.DeviceCategory));
            if (modelListView!=null)
                return modelColumn;
            return ((IModelDetailView) View.Model).Items.OfType<IModelPropertyEditor>().FirstOrDefault(
                    item => item.ModelMember.MemberInfo.Name == nameof(ModelDifferenceObject.DeviceCategory));
        }
    }
}
