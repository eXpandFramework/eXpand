using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    /// <summary>
    /// on load of ActiveUserAspect
    /// </summary>
    public class CombineActiveUserDifferenceWithUserModelController : ViewController
    {
        public CombineActiveUserDifferenceWithUserModelController(){
            TargetViewType=ViewType.DetailView;
            TargetObjectType = typeof (UserModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
            CombineWithApplicationUserDiffs();
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        protected virtual void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            CombineWithApplicationUserDiffs();
        }

        public void CombineWithApplicationUserDiffs(){
            var userAspectObjectQuery = new QueryUserModelDifferenceObject(View.ObjectSpace.Session);
            ModelDifferenceObject differenceObject = userAspectObjectQuery.GetActiveModelDifference(Application.GetType().FullName);
            if (ReferenceEquals(differenceObject, View.CurrentObject))
            {
                //((UserModelDifferenceObject)View.CurrentObject).Model.AddLayer(
                    //((ModelApplicationBase)Application.Model).LastLayer);
            }
        }
    }
    }

