using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Fasterflect;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Model {
    [ModelInterfaceImplementor(typeof(IModelClassEnableUnboundColumnCreation), "ModelClass")]
    public interface IModelListViewEnableUnboundColumnCreation : IModelClassEnableUnboundColumnCreation {
    }

    public interface IModelClassEnableUnboundColumnCreation:IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool EnableUnboundColumnCreation { get; set; }
    }

    public interface IModelColumnUnbound : IModelColumn {
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [Required]
        [Category("eXpand.Unbound")]
        [RefreshProperties(RefreshProperties.All)]
        [ModelValueCalculator("UnboundPropertyEditorType")]
        [Browsable(false)]
        new Type PropertyEditorType { get; set; }

        [DataSourceProperty("UnboundPropertyEditorTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [Required]
        [Category("eXpand.Unbound")]
        [RefreshProperties(RefreshProperties.All)]
        Type UnboundPropertyEditorType { get; set; }

        [Browsable(false)]
        IEnumerable<Type> UnboundPropertyEditorTypes { get; }

        [Category("eXpand.Unbound")]
        bool ShowUnboundExpressionMenu { get; set; }
        [Category("eXpand.Unbound")]
        [Required]
        string UnboundExpression { get; set; }
        [ModelBrowsable(typeof(ModelPropertyEditorTypeVisibilityCalculator))]
        new string PropertyName { get; set; }
        [Localizable(true)]
        [Description("Specifies the caption of the current Property Editor.")]
        [Category("eXpand.Unbound")]
        [Required]
        new string Caption { get; set; }
        [Category("eXpand.Unbound")]
        [RefreshProperties(RefreshProperties.All)]
        UnboundType UnboundType { get; set; }
    }

    public enum UnboundType {
        Object,
        Integer,
        Decimal,
        DateTime,
        String,
        Boolean,
    }

    public class ModelPropertyEditorTypeVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return propertyName != "PropertyEditorType" && propertyName != "PropertyName";
        }
    }

    public class UnboundColumnInfoCalculator : EditorInfoCalculator<IModelColumnUnbound> {
        protected override Type GetElementType(IModelColumnUnbound modelNode) {
            switch (modelNode.UnboundType) {
                case UnboundType.Boolean:
                    return typeof (bool);
                case UnboundType.DateTime:
                    return typeof (DateTime);
                case UnboundType.Decimal:
                    return typeof (decimal);
                case UnboundType.Integer:
                    return typeof (int);
                case UnboundType.String:
                    return typeof (string);
            }
            return typeof(object);
        }

        protected override EditorTypeRegistrations GetEditorTypeRegistrations(IModelColumnUnbound modelNode) {
            return ((IModelSources)modelNode.Application).EditorDescriptors.PropertyEditorRegistrations;
        }

        protected override bool IsCompatibleEditorDescriptor(EditorDescriptor editorDescriptor) {
            return editorDescriptor is PropertyEditorDescriptor;
        }

        internal static bool GetIsDefaultEditor(IAliasRegistration aliasRegistration) {
            if (aliasRegistration != null) {
                return aliasRegistration.ElementType == typeof(Object) && (aliasRegistration.IsDefaultAlias || !aliasRegistration.HasCompatibleDelegate);
            }
            return false;
        }

        protected override Type GetDefaultEditorTypeFromModel(IEditorTypeRegistration registration, IAliasRegistration aliasRegistration,
                                                              IModelNode modelNode) {
            var memberEditorInfoCalculator = new MemberEditorInfoCalculator();
            var parameterTypes = new[]{typeof (IEditorTypeRegistration), typeof (IAliasRegistration),typeof(IModelNode)};
            var callMethod = memberEditorInfoCalculator.CallMethod("GetDefaultEditorTypeFromModel", parameterTypes,Flags.NonPublic|Flags.Instance,registration,aliasRegistration,modelNode);
            return (Type) callMethod;
        }
    }
    [DomainLogic(typeof(IModelColumnUnbound))]
    public class ModelColumnUnboundLogic {
        static readonly UnboundColumnInfoCalculator _unboundColumnInfoCalculator=new UnboundColumnInfoCalculator();
        public static Type Get_UnboundPropertyEditorType(IModelColumnUnbound columnUnbound) {
            return _unboundColumnInfoCalculator.GetEditorType(columnUnbound);
        }

        public static IEnumerable<Type> Get_UnboundPropertyEditorTypes(IModelColumnUnbound modelMember) {
            return _unboundColumnInfoCalculator.GetEditorsType(modelMember);
        }

        public static string Get_PropertyName(IModelColumnUnbound columnUnbound) {
            return ((IModelListView)columnUnbound.Parent.Parent).ModelClass.KeyProperty;
        }
    }

    public class UnboundColumnController:ViewController<ListView>,IModelExtender {
        readonly SimpleAction _unboundColumnAction;

        public UnboundColumnController() {
            _unboundColumnAction = new SimpleAction(this, "Unbound column",PredefinedCategory.View);
            _unboundColumnAction.Execute+=UnboundColumnActionOnExecute;
            _unboundColumnAction.Active["UnboundColumnController"] = false;
        }

        void UnboundColumnActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var showViewParameters = simpleActionExecuteEventArgs.ShowViewParameters;
            var objectSpace = Application.CreateObjectSpace();
            var detailView = Application.CreateDetailView(objectSpace, new UnboundColumnParemeter());
            showViewParameters.CreatedView=detailView;
            showViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            var dialogController = new DialogController{SaveOnAccept = true};
            dialogController.AcceptAction.Execute+=AcceptActionOnExecute;
            showViewParameters.Controllers.Add(dialogController);
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var view = ((SimpleAction) sender).Controller.Frame.View;
            Validator.RuleSet.Validate(view.ObjectSpace, simpleActionExecuteEventArgs.CurrentObject, ContextIdentifier.Save);
            var unboundColumnParemeter = ((UnboundColumnParemeter) simpleActionExecuteEventArgs.CurrentObject);
            var modelColumnUnbound = View.Model.Columns.AddNode<IModelColumnUnbound>(unboundColumnParemeter.ColumnName);
            modelColumnUnbound.Caption = unboundColumnParemeter.ColumnName;
            modelColumnUnbound.UnboundExpression = " ";
            modelColumnUnbound.ShowUnboundExpressionMenu = true;
            modelColumnUnbound.Index = 0;
            modelColumnUnbound.UnboundType=unboundColumnParemeter.UnboundType;
            AddColumn(modelColumnUnbound);
        }


        protected virtual void AddColumn(IModelColumnUnbound modelColumnUnbound) {
            
        }

        protected override void OnActivated() {
            base.OnActivated();
            _unboundColumnAction.Active["UnboundColumnController"] = ((IModelListViewEnableUnboundColumnCreation)View.Model).EnableUnboundColumnCreation;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewEnableUnboundColumnCreation>();
            extenders.Add<IModelClass, IModelClassEnableUnboundColumnCreation>();
        }

        [ModelDefault("Caption", "Unbound column name")]
        [NonPersistent]
        public class UnboundColumnParemeter {
            [RuleRequiredField]
            public string ColumnName { get; set; }
            public UnboundType UnboundType { get; set; }
        }
    }
}