using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class XafLayoutControlModelAdapterController:ModelAdapterController,IModelExtender {
        public XafLayoutControlModelAdapterController(){
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var layoutControl = ((WinLayoutManager)(((DetailView) View).LayoutManager)).Container;
            var modelXafLayoutControl = ((IModelDetailViewXafLayoutControl) View.Model).XafLayoutControl;
            foreach (var modelAdapter in modelXafLayoutControl.ModelAdapters){
                new ObjectModelSynchronizer(layoutControl, modelAdapter.ModelAdapter).ApplyModel();
            }
            new ObjectModelSynchronizer(layoutControl, modelXafLayoutControl).ApplyModel();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelDetailView, IModelDetailViewXafLayoutControl>();
            var builder = new InterfaceBuilder(extenders);
            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(XafLayoutControl).Name));

            builder.ExtendInteface<IModelXafLayoutControl, XafLayoutControl>(assembly);

        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(XafLayoutControl)) {
                Act = info => (info.DXFilter(new[] { typeof(BaseLayoutOptions) }, typeof(object)))
            };
        }

    }
    [ModelAbstractClass]
    public interface IModelDetailViewXafLayoutControl:IModelDetailView{
        IModelXafLayoutControl XafLayoutControl { get; }
    }

    public interface IModelXafLayoutControl : IModelModelAdapter {
        IModelXafLayoutControlModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelXafLayoutControlAdaptersNodeGenerator))]
    public interface IModelXafLayoutControlModelAdapters : IModelList<IModelXafLayoutControlModelAdapter>, IModelNode {

    }

    public class ModelXafLayoutControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelXafLayoutControl, IModelXafLayoutControlModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelXafLayoutControlModelAdapter : IModelCommonModelAdapter<IModelXafLayoutControl> {
    }

    [DomainLogic(typeof(IModelXafLayoutControlModelAdapter))]
    public class ModelXafLayoutControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelXafLayoutControl> {
        public static IModelList<IModelXafLayoutControl> Get_ModelAdapters(IModelXafLayoutControlModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
