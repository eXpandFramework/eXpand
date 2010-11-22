using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.Core;
using System.Linq;

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

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            _synchronizeActiveUserDifferenceWithUserModel = ((IModelOptionsSynchronizeActiveUserDifferenceWithUserModel)Application.Model.Options).SynchronizeActiveUserDifferenceWithUserModel;
            if (_synchronizeActiveUserDifferenceWithUserModel)
            {
                ISupportMasterModel supportMasterModel = View.GetItems<ISupportMasterModel>().Single();
                supportMasterModel.ModelCreated += (sender, args) => CombineWithApplicationUserDiffs(supportMasterModel.MasterModel);
            }
            
        }


        public void CombineWithApplicationUserDiffs(ModelApplicationBase masterModel){
            var userAspectObjectQuery = new QueryUserModelDifferenceObject(((ObjectSpace)View.ObjectSpace).Session);
            ModelDifferenceObject differenceObject = userAspectObjectQuery.GetActiveModelDifference(Application.GetType().FullName,null);
            if (ReferenceEquals(differenceObject, View.CurrentObject)) {
                new ModelXmlReader().ReadFromModel(masterModel.LastLayer, ((ModelApplicationBase)Application.Model).LastLayer);
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

