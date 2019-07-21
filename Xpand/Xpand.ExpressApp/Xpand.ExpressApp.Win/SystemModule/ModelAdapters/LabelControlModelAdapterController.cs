//using System;
//using System.Linq.Expressions;
//using System.Reflection;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.XtraEditors;
//using Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors;
//using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Win.SystemModule.ModelAdapters{
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemLabelControl : IModelMemberViewItem {
//        [ModelBrowsable(typeof(LabelMemberViewItemVisibilityCalculator))]
//        IModelLabelControl LabelControl { get; }
//    }
//
//    public class LabelMemberViewItemVisibilityCalculator : EditorTypeVisibilityCalculator<LabelControlPropertyEditor, IModelMemberViewItem> {
//    }
//
//    public interface IModelLabelControl : IModelModelAdapter {
//        IModelLabelControlModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelLabelControlAdaptersNodeGenerator))]
//    public interface IModelLabelControlModelAdapters : IModelList<IModelLabelControlModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelLabelControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelLabelControl, IModelLabelControlModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelLabelControlModelAdapter : IModelCommonModelAdapter<IModelLabelControl> {
//    }
//
//    [DomainLogic(typeof(IModelLabelControlModelAdapter))]
//    public class ModelLabelControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelLabelControl> {
//        public static IModelList<IModelLabelControl> Get_ModelAdapters(IModelLabelControlModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//    public class LabelControlModelAdapterController : PropertyEditorControlAdapterController<IModelMemberViewItemLabelControl,IModelLabelControl,LabelControlPropertyEditor> {
//        protected override Expression<Func<IModelMemberViewItemLabelControl, IModelModelAdapter>> GetControlModel(IModelMemberViewItemLabelControl modelMemberViewItemFilterControl){
//            return control => control.LabelControl;
//        }
//
//        protected override Type GetControlType(){
//            return typeof (LabelControl);
//        }
//
//        protected override void ExtendingModelInterfaces(InterfaceBuilder builder, Assembly assembly, ModelInterfaceExtenders extenders){
//            var calcType = builder.CalcType(typeof(LabelControlAppearanceObject), assembly);
//            extenders.Add(calcType, typeof(IModelAppearanceFont));
//        }
//
//    }
//}