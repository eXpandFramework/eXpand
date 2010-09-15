using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference.Controllers
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
                var model = ((UserModelDifferenceObject)View.CurrentObject).GetModel((ModelApplicationBase) Application.Model);
                new ModelXmlReader().ReadFromModel(model, ((ModelApplicationBase)Application.Model).LastLayer);
                ObjectSpace.SetModified(userAspectObjectQuery);
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

