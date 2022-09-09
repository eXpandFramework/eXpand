using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Registration {
    public class ManageUsersOnLogonController : ViewController<DetailView>,IModelExtender {
        public event EventHandler<ParameterEventArgs> CustomProccessLogonParameter;
        public event EventHandler<ParameterEventArgs> CustomProccessedLogonParameter;
        public event EventHandler<ParameterEventArgs> CustomCancelLogonParameter;

        protected virtual void OnCustomCancelLogonParameter(ParameterEventArgs e){
            var handler = CustomCancelLogonParameter;
            handler?.Invoke(this, e);
        }

        protected virtual void OnCustomProccessedLogonParameter(ParameterEventArgs e){
            var handler = CustomProccessedLogonParameter;
            handler?.Invoke(this, e);
        }

        public event EventHandler<CustomActiveKeyArgs> CustomActiveKey;

        protected virtual void OnCustomActiveKey(CustomActiveKeyArgs e) {
            var handler = CustomActiveKey;
            handler?.Invoke(this, e);
        }


        protected virtual void OnCustomProccessLogonParameter(ParameterEventArgs e) {
            var handler = CustomProccessLogonParameter;
            handler?.Invoke(this, e);
        }


        protected const string LogonActionParametersActiveKey = "Active for ILogonActionParameters only";
        public const string EmailPattern = @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";

        public ManageUsersOnLogonController() {
            RegisterUserAction = CreateLogonSimpleAction("RegisterUser", "RegisterUserCategory", "Register User", "BO_User", "Register a new user within the system", typeof(RegisterUserParameters));
            RestorePasswordAction = CreateLogonSimpleAction("RestorePassword", "RestorePasswordCategory", "Restore Password", "Action_ResetPassword", "Restore forgotten login information", typeof(RestorePasswordParameters));
        }

        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            var customActiveKeyArgs = new CustomActiveKeyArgs(view);
            OnCustomActiveKey(customActiveKeyArgs);
            var activeKey = !SecuritySystem.IsAuthenticated;
            if (customActiveKeyArgs.Handled){
                activeKey = customActiveKeyArgs.Handled;
            }
            Active[ControllerActiveKey] = activeKey;
        }
                
        private SimpleAction CreateLogonSimpleAction(string id, string category, string caption, string imageName, string toolTip, Type parametersType) {
            var action = new SimpleAction(this, id, category) {
                Caption = caption,
                ImageName = imageName,
                PaintStyle = ActionItemPaintStyle.Image,
                ToolTip = toolTip
            };
            action.Execute += CreateParametersView;
            action.Tag = parametersType;
            return action;
        }
        
        private void CreateParametersView(object sender, SimpleActionExecuteEventArgs e) {
            Application.CallMethod("EnsureShowViewStrategy",Flags.InstancePrivate);
            CreateParametersViewCore(e);
        }
        
        protected virtual void CreateParametersViewCore(SimpleActionExecuteEventArgs e) {
            var parametersType = e.Action.Tag as Type;
            var detailView = Application.CreateDetailView(Application.CreateObjectSpace(parametersType), parametersType.CreateInstance());
            detailView.ViewEditMode = ViewEditMode.Edit;
            e.ShowViewParameters.CreatedView = detailView;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.Current;
            var dialogController = Frame.GetController<LogonController>();

            Frame.RegisterController(new ActionDataHolder(dialogController.AcceptAction.Caption, dialogController.AcceptAction.ToolTip));
            dialogController.AcceptAction.Caption = e.Action.Caption;
            dialogController.AcceptAction.ToolTip = e.Action.ToolTip;
            dialogController.AcceptAction.Executing+=AcceptActionOnExecuting;
            var cancelAction = dialogController.CancelAction;
            cancelAction.Executing+=CancelActionOnExecuting;
            cancelAction.ActivateKey("Web application logon");
        }

        class ActionDataHolder:Controller{
            public ActionDataHolder(string caption, string tooltip){
                Caption = caption;
                Tooltip = tooltip;
            }

            public string Caption { get; }

            public string Tooltip { get; }
        }

        private void CancelActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            CancelParameters(View.CurrentObject as ILogonParameters);
            var actionBase = (ActionBase)sender;
            var window = ((DialogController)actionBase.Controller).Window;
            ShowCallerView(actionBase, window, cancelEventArgs);
        }

        void AcceptActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            var actionBase = (ActionBase) sender;
            var window = ((DialogController) actionBase.Controller).Window;

            var currentObject = window.View.CurrentObject;
            Validator.RuleSet.Validate(window.View.ObjectSpace, currentObject, ContextIdentifier.Save);

            AcceptParameters(View.CurrentObject as ILogonParameters);
            ShowCallerView(actionBase,window,cancelEventArgs);
        }

        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        private void ShowCallerView(ActionBase actionBase, Window window, CancelEventArgs cancelEventArgs){
            var dialogController = window.GetController<DialogController>();
            var actionDataHolder = window.GetController<ActionDataHolder>();
            dialogController.AcceptAction.Caption = actionDataHolder.Caption;
            dialogController.AcceptAction.ToolTip = actionDataHolder.Tooltip;
            cancelEventArgs.Cancel = true;
            actionBase.Executing -= AcceptActionOnExecuting;
            var detailView = Application.CreateDetailView(Application.CreateObjectSpace(SecuritySystem.LogonParameters.GetType()), SecuritySystem.LogonParameters);
            detailView.ViewEditMode=ViewEditMode.Edit;
            var showViewParameters = new ShowViewParameters{
                CreatedView = detailView,
                TargetWindow = TargetWindow.Current,
                Context = TemplateContext.PopupWindow
            };
            Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, actionBase));
        }

        protected virtual void AcceptParameters(ILogonParameters parameters) {
            var eventArgs = new ParameterEventArgs(parameters);
            OnCustomProccessLogonParameter(eventArgs);
            if (!eventArgs.Handled){
                parameters.Process(Application,ObjectSpace);
            }
        }

        protected virtual void CancelParameters(ILogonParameters parameters) {
            var parameterEventArgs = new ParameterEventArgs(parameters);
            OnCustomCancelLogonParameter(parameterEventArgs);
            if (!parameterEventArgs.Handled)
                Application.LogOff();
        }
        
        protected virtual bool GetLogonParametersActiveState() {
            return View?.ObjectTypeInfo != null && View.ObjectTypeInfo.Implements<ILogonParameters>();
        }

        public SimpleAction RestorePasswordAction { get; }

        public SimpleAction RegisterUserAction { get; }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsRegistration>();
            extenders.Add<IModelRegistrationEnabled, IModelRegistration>();
            extenders.Add<IModelRegistrationEnabled, IModelRegistrationActivation>();
        }
    }


    public class CustomActiveKeyArgs:HandledEventArgs {
        public CustomActiveKeyArgs(View view) {
            View = view;
            Handled = false;
        }

        public View View { get; }
    }

    public class ParameterEventArgs:HandledEventArgs {
        public ParameterEventArgs(ILogonParameters parameters) {
            Parameters = parameters;
        }

        public ILogonParameters Parameters { get; }
    }
}