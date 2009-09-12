using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    /// <summary>
    /// on load of ActiveUserAspect
    /// </summary>
    public partial class CombineUserModelWithActiveUserDifferenceController : ViewController
    {
        public CombineUserModelWithActiveUserDifferenceController()
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
        }

        protected  virtual void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            var userAspectObjectQuery = new QueryUserModelDifferenceObject(View.ObjectSpace.Session);
            var applicationUniqueName = Application as IApplicationUniqueName;
            if (applicationUniqueName != null){
                ModelDifferenceObject differenceObject =userAspectObjectQuery.GetActiveModelDifference(applicationUniqueName.UniqueName);
                if (ReferenceEquals(differenceObject, View.CurrentObject)){
                    var dictionaryCombiner = new DictionaryCombiner(Application.Model);
                    dictionaryCombiner.AddAspects(((UserModelDifferenceObject)View.CurrentObject));                
                }
            }
        }
    }
}
