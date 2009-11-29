using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    /// <summary>
    /// on load of ActiveUserAspect
    /// </summary>
    public partial class CombineActiveUserDifferenceWithUserModelController : ViewController
    {
        public CombineActiveUserDifferenceWithUserModelController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.DetailView;
            TargetObjectType = typeof (UserModelDifferenceObject);
        }

        
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
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
                var o = ((UserModelDifferenceObject)View.CurrentObject);
                Dictionary dict = o.GetCombinedModel();
                dict.CombineWith(Application.Model.GetDiffs());
                o.Model = dict.GetDiffs();
            }
        }
    }
    }

