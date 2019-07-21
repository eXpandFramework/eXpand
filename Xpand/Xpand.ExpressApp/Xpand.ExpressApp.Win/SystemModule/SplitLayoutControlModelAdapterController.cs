using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule {
//    public class SplitContainerControlModelAdapterController : ModelAdapterController, IModelExtender {
//        public SplitContainerControlModelAdapterController() {
//            TargetViewType = ViewType.ListView;
//        }
//
//        protected override void OnViewControlsCreated() {
//            base.OnViewControlsCreated();
//            var splitContainerControl = ((((ListView)View).LayoutManager)).Container as SplitContainerControl;
//            if (splitContainerControl!=null){
//                var modelSplitContainerControl = ((IModelListViewSplitContainerControl) ((IModelListView) View.Model).SplitLayout).SplitContainerControl;
//                new ObjectModelSynchronizer(splitContainerControl, modelSplitContainerControl).ApplyModel();
//            }
//        }
//
//        protected override void OnDeactivated(){
//            base.OnDeactivated();
//            var listView = ((ListView)View);
//            var splitContainerControl = ((listView.LayoutManager)).Container as SplitContainerControl;
//            if (splitContainerControl != null) {
//                var modelListViewSplitContainerControl = ((IModelListViewSplitContainerControl)listView.Model.SplitLayout).SplitContainerControl;
//                var splitCollapsePanel = modelListViewSplitContainerControl.GetValue<SplitCollapsePanel?>(nameof(SplitContainerControl.CollapsePanel));
//                if (splitCollapsePanel.HasValue&& splitCollapsePanel != SplitCollapsePanel.None)
//                    modelListViewSplitContainerControl.SetValue(nameof(SplitContainerControl.Collapsed), new bool?(splitContainerControl.Collapsed));
//            }
//        }
//
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            extenders.Add<IModelListViewSplitLayout, IModelListViewSplitContainerControl>();
//            var builder = new InterfaceBuilder(extenders);
//            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(SplitContainerControl).Name));
//
//            builder.ExtendInteface<IModelSplitContainerControl, SplitContainerControl>(assembly);
//
//        }
//
//        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            yield return new InterfaceBuilderData(typeof(SplitContainerControl)) {
//                Act = info => (info.DXFilter(new[] { typeof(SplitGroupPanel) }, typeof(object)))
//            };
//        }
//
//    }
//    [ModelAbstractClass]
//    public interface IModelListViewSplitContainerControl : IModelListViewSplitLayout {
//        IModelSplitContainerControl SplitContainerControl { get; }
//    }

//    public interface IModelSplitContainerControl : IModelModelAdapter {
//        IModelSplitContainerControlModelAdapters ModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelSplitContainerControlAdaptersNodeGenerator))]
//    public interface IModelSplitContainerControlModelAdapters : IModelList<IModelSplitContainerControlModelAdapter>, IModelNode {
//
//    }

//    public class ModelSplitContainerControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelSplitContainerControl, IModelSplitContainerControlModelAdapter> {
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelSplitContainerControlModelAdapter : IModelCommonModelAdapter<IModelSplitContainerControl> {
//    }

//    [DomainLogic(typeof(IModelSplitContainerControlModelAdapter))]
//    public class ModelSplitContainerControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelSplitContainerControl> {
//        public static IModelList<IModelSplitContainerControl> Get_ModelAdapters(IModelSplitContainerControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
}
