using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    public interface IModelOptionsSynchronizeActiveUserDifferenceWithUserModel:IModelOptions
    {
        [Category("eXpand.ModelDifference")]
        [Description("When open active user difference object's detailview current user diffs are going to be shown as well")]
        [DefaultValue(true)]
        bool SynchronizeActiveUserDifferenceWithUserModel { get; set; }
    }
    public class SynchronizeActiveUserDifferenceWithUserModelController : ViewController<DetailView>,IModelExtender
    {
        bool _synchronizeActiveUserDifferenceWithUserModel;

        public SynchronizeActiveUserDifferenceWithUserModelController(){
            TargetViewType=ViewType.DetailView;
            TargetObjectType = typeof (UserModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            _synchronizeActiveUserDifferenceWithUserModel = ((IModelOptionsSynchronizeActiveUserDifferenceWithUserModel)Application.Model.Options).SynchronizeActiveUserDifferenceWithUserModel;
            if (_synchronizeActiveUserDifferenceWithUserModel) {
                View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
                CombineWithApplicationUserDiffs();
            }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (_synchronizeActiveUserDifferenceWithUserModel)
                View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        protected virtual void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            CombineWithApplicationUserDiffs();
        }

        public void CombineWithApplicationUserDiffs(){
            var userAspectObjectQuery = new QueryUserModelDifferenceObject(View.ObjectSpace.Session);
            ModelDifferenceObject differenceObject = userAspectObjectQuery.GetActiveModelDifference(Application.GetType().FullName,null);
            if (ReferenceEquals(differenceObject, View.CurrentObject)) {
                var model = ((UserModelDifferenceObject)View.CurrentObject).Model;
                new ModelXmlReader().ReadFromString(model, Application.CurrentAspectProvider.CurrentAspect, ((ModelApplicationBase)Application.Model).LastLayer.Xml);
                differenceObject.Model = differenceObject.Model.Clone();
                ObjectSpace.CommitChanges();
            }
        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsSynchronizeActiveUserDifferenceWithUserModel>();
        }

        #endregion
    }
    }

