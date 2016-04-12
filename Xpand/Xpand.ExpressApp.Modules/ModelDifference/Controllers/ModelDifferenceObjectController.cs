using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Controllers
{
    public class ModelDifferenceObjectController : WindowController
    {
        private SimpleAction _ActionModelDifference;
        public ModelDifferenceObjectController()
        {
            TargetWindowType = WindowType.Main;

            _ActionModelDifference = new SimpleAction(this, "Application Difference", DevExpress.Persistent.Base.PredefinedCategory.Tools)
            {
                ImageName = "ModelEditor_Action_Modules"
            };

            _ActionModelDifference.Execute += _ActionUserRole_Execute;
        }

        private void _ActionUserRole_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            string listViewId = Application.FindListViewId(typeof(ModelDifferenceObject));
            CollectionSourceBase cs = Application.CreateCollectionSource(os, typeof(ModelDifferenceObject), listViewId);

            e.ShowViewParameters.CreatedView = Application.CreateListView(listViewId, cs, true);
        }
        protected override void UpdateActionActivity(ActionBase action)
        {
            base.UpdateActionActivity(action);

            bool hasRights = SecuritySystem.IsGranted(new EditModelPermission());
            action.Active.SetItemValue("Has Rights", hasRights);
        }
        public SimpleAction ActionModelDifference
        {
            get { return _ActionModelDifference; }
        }
    }
}