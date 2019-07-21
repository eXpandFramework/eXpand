using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraRichEdit;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule.ModelAdapters{
//    [ModelAbstractClass]
//    public interface IModelMemberViewItemRichEdit : IModelMemberViewItem {
//        [ModelBrowsable(typeof(ModelMemberViewItemRichEditVisibilityCalculator))]
//        IModelRichEditEx RichEdit { get; }
//    }

//    [ModuleUser(typeof(IRichEditUser))]
    public interface IModelRichEditEx:IModelNode {//: IModelModelAdapter {
        [DefaultValue("rtf")]
        [Category(AttributeCategoryNameProvider.Xpand)]
        string HighLightExtension { get; set; }
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool PrintXML { get; set; }
        [DefaultValue(true)]
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool ShowToolBars { get; set; }
        [DefaultValue("Text")]
        [Category(AttributeCategoryNameProvider.Xpand)]
        string ControlBindingProperty { get; set; }
//        IModelRichEditModelAdapters ModelAdapters { get; }
    }

//    public interface IRichEditUser {
//    }

//    [ModelNodesGenerator(typeof(ModelRichEditAdaptersNodeGenerator))]
//    public interface IModelRichEditModelAdapters : IModelList<IModelRichEditModelAdapter>, IModelNode {
//
//    }

//    public class ModelRichEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRichEdit, IModelRichEditModelAdapter> {
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelRichEditModelAdapter : IModelCommonModelAdapter<IModelRichEdit> {
//    }

//    [DomainLogic(typeof(IModelRichEditModelAdapter))]
//    public class ModelDashboardViewerModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRichEdit> {
//        public static IModelList<IModelRichEdit> Get_ModelAdapters(IModelRichEditModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }

    [DomainLogic(typeof(IModelRichEditEx))]
    public class ModelRichEditDomainLogic {
        public static string Get_ControlBindingProperty(IModelRichEditEx modelRichEdit) {
            return GetValue(modelRichEdit, attribute => attribute.ControlBindingProperty) as string;
        }

        public static bool Get_ShowToolBars(IModelRichEditEx modelRichEdit) {
            var value = GetValue(modelRichEdit, attribute => attribute.ShowToolBars);
            return value != null && (bool)value;
        }

        public static bool Get_PrintXML(IModelRichEditEx modelRichEdit) {
            var value = GetValue(modelRichEdit, attribute => attribute.PrintXML);
            return value != null && (bool)value;
        }

        public static string Get_HighLightExtension(IModelRichEditEx modelRichEdit) {
            return GetValue(modelRichEdit, attribute => attribute.HighLightExtension) as string;
        }

        private static object GetValue(IModelRichEditEx modelRichEdit, Func<RichEditPropertyEditorAttribute, object> func) {
            if (modelRichEdit.Parent is IModelMemberViewItem richEdit) {
                var editorType = richEdit.PropertyEditorType;
                if (typeof(RichEditWinPropertyEditor).IsAssignableFrom(editorType)) {
                    var editorAttribute = editorType.GetCustomAttributes(typeof(RichEditPropertyEditorAttribute), false)
                        .Cast<RichEditPropertyEditorAttribute>().First();
                    return func(editorAttribute);
                }
                return "rtf";
            }
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RichEditPropertyEditorAttribute : Attribute {
        public RichEditPropertyEditorAttribute(string highLightExtension, bool showToolBars, bool printXML, string controlBindingProperty) {
            HighLightExtension = highLightExtension;
            ShowToolBars = showToolBars;
            PrintXML = printXML;
            ControlBindingProperty = controlBindingProperty;
        }

        public bool PrintXML { get; }

        public string ControlBindingProperty { get; }

        public string HighLightExtension { get; }

        public bool ShowToolBars { get; }
    }

    public class ModelMemberViewItemRichEditVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return typeof(RichEditWinPropertyEditor).IsAssignableFrom(((IModelMemberViewItem)node).PropertyEditorType);
        }
    }
//    public class RichEditModelAdapterController : PropertyEditorControlAdapterController<IModelMemberViewItemRichEdit, IModelRichEdit,RichEditWinPropertyEditor> {
//
//        protected override object GetPropertyEditorControl(RichEditWinPropertyEditor richEditWinPropertyEditor){
//            return richEditWinPropertyEditor.Control.RichEdit;
//        }
//
//        protected override Expression<Func<IModelMemberViewItemRichEdit, IModelModelAdapter>> GetControlModel(IModelMemberViewItemRichEdit modelMemberViewItemFilterControl){
//            return edit => edit.RichEdit;
//        }
//
//        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData(){
//            return Enumerable.Empty<InterfaceBuilderData>();
////            var interfaceBuilderData = new InterfaceBuilderData(typeof(RichEdit)) {
////                Act = info => {
////                    if (info.PropertyType==typeof(RichEditRulerVisibility))
////                        info.AddAttribute(new DefaultValueAttribute(RichEditRulerVisibility.Hidden));
////                    else if (info.PropertyType==typeof(RichEditViewType))
////                        info.AddAttribute(new DefaultValueAttribute(RichEditViewType.Simple));
////                    return info.Name != "Undo" && info.DXFilter();
////                }
////            };
////            interfaceBuilderData.ReferenceTypes.AddRange(new[] { typeof(CriteriaOperator), typeof(DocumentCapability) });
////            yield return interfaceBuilderData;
//        }
//
//        protected override Type GetControlType(){
//            return typeof (RichEdit);
//        }
//    }
}