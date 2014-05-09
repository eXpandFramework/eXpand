using System.Collections.Generic;
using DevExpress.ExpressApp;
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
            new ObjectModelSynchronizer(layoutControl, ((IModelDetailViewXafLayoutControl) View.Model).XafLayoutControl).ApplyModel();
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
         
    }
}
