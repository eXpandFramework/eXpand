using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;

namespace eXpand.ExpressApp.SystemModule
{
    public enum LookUpListSearch
    {
        Default,
        AlwaysEnable
    }

    public interface IModelListViewLookUpListSearch
    {
        [Category("eXpand")]
        LookUpListSearch LookUpListSearch { get; set; }
    }

    public class LookUpListSearchAlwaysEnableController : BaseViewController, IModelExtender
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (Frame.Template is ILookupPopupFrameTemplate)
            {
                if (((IModelListViewLookUpListSearch)View.Model).LookUpListSearch == LookUpListSearch.AlwaysEnable)
                    ((ILookupPopupFrameTemplate)Frame.Template).IsSearchEnabled = true;
            }
        }

        public override void UpdateModel(IModelApplication applicationModel)
        {

            base.UpdateModel(applicationModel);
            return;
            IEnumerable<string> enumerable = applicationModel.BOModel
                .Where(wrapper => typeof(ICategorizedItem).IsAssignableFrom(wrapper.TypeInfo.Type))
                .Select(wrapper => wrapper.TypeInfo.FullName);

            foreach (IModelListView modelView in applicationModel.Views.Where(
                wrapper => wrapper.Id.EndsWith("_LookupListView") && enumerable.Contains(wrapper.ModelClass.Name)))
                ((IModelListViewLookUpListSearch)modelView).LookUpListSearch = LookUpListSearch.AlwaysEnable;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewLookUpListSearch>();
        }
    }
}