using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core.Design;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web.ASPxMenu;
using DevExpress.Web.ASPxPanel;

namespace Xpand.ExpressApp.Web.Templates{
    [ToolboxItem(false)]
    public class ASPxMenuNavigationActionContainer : ASPxPanel, IActionContainer
        /*, ISupportNavigationActionContainerTesting*/ /*, ITestableEx*/, ISupportCallbackStartupScriptRegistering
        /*, ISupportAdditionalParametersTestControl*/{
        private IWebNavigationControl _mainNavControl;
        private SingleChoiceAction _singleChoiceAction;

        public ASPxMenuNavigationActionContainer(){
            ContainerId = NavigationHelper.DefaultContainerId;
        }

        public override void Dispose(){
            UnsubscribeFromCallbackStartupScriptRegistering();
            if (_mainNavControl != null){
                _mainNavControl.Control.Unload -= Control_Unload;
                var control = _mainNavControl as IDisposable;
                if (control != null){
                    control.Dispose();
                }
            }
            RegisterCallbackStartupScript = null;
            base.Dispose();
        }

        private void SubscribeToCallbackStartupScriptRegistering(){
            var startupScriptRegistering = _mainNavControl as ISupportCallbackStartupScriptRegistering;
            if (startupScriptRegistering != null){
                startupScriptRegistering.RegisterCallbackStartupScript +=
                    startupScriptRegistering_RegisterCallbackStartupScript;
            }
        }

        private void UnsubscribeFromCallbackStartupScriptRegistering(){
            var startupScriptRegistering = _mainNavControl as ISupportCallbackStartupScriptRegistering;
            if (startupScriptRegistering != null){
                startupScriptRegistering.RegisterCallbackStartupScript -=
                    startupScriptRegistering_RegisterCallbackStartupScript;
            }
        }

        private void startupScriptRegistering_RegisterCallbackStartupScript(object sender,
            RegisterCallbackStartupScriptEventArgs e){
            OnRegisterCallbackStartupScript(e);
        }

        protected virtual IWebNavigationControl CreateNavigationControl(NavigationStyle controlStyle){
            switch (controlStyle){
                case NavigationStyle.TreeList:
                    return new TreeViewNavigationControl();
                case NavigationStyle.NavBar:{
                    var navControl = new ASPxMenuNavigationControl();
                    ((ASPxMenu) navControl.Control).Width = Width;
                    return navControl;
                }
                default:
                    return null;
            }
        }

        protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e){
            if (RegisterCallbackStartupScript != null){
                RegisterCallbackStartupScript(this, e);
            }
        }

        protected override void OnLoad(EventArgs e){
            base.OnLoad(e);
            WebActionContainerHelper.TryRegisterActionContainer(this, new IActionContainer[]{this});
        }

        public void Register(ActionBase action, NavigationStyle navigationStyle){
            UnsubscribeFromCallbackStartupScriptRegistering();
            var disposable = _mainNavControl as IDisposable;
            if (disposable != null){
                disposable.Dispose();
            }
            Controls.Clear();
            if (navigationStyle == NavigationStyle.NavBar){
                CssClass += " NavBarLiteAC";
            }
            _mainNavControl = CreateNavigationControl(navigationStyle);
            _mainNavControl.Control.Unload += Control_Unload;
            SubscribeToCallbackStartupScriptRegistering();
            _mainNavControl.SetNavigationActionItems(((ChoiceActionBase) action).Items, (SingleChoiceAction) action);
            Controls.Add(_mainNavControl.Control);
            _singleChoiceAction = action as SingleChoiceAction;
        }

        private void Control_Unload(object sender, EventArgs e){
            OnControlInitialized(sender as Control);
        }

        protected void OnControlInitialized(Control navControl){
            if (ControlInitialized != null){
                ControlInitialized(this, new ControlInitializedEventArgs(navControl));
            }
        }

        #region IActionContainer Members

        [DefaultValue(NavigationHelper.DefaultContainerId), TypeConverter(typeof (ContainerIdConverter)),
         Category("Design")]
        public string ContainerId { get; set; }

        public void Register(ActionBase action){
            var navigationStyle = NavigationStyle.NavBar;
            if (action.Application != null){
                navigationStyle = NavigationHelper.GetControlStyle(action.Application.Model);
            }
            Register(action, navigationStyle);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReadOnlyCollection<ActionBase> Actions{
            get { return new ReadOnlyCollection<ActionBase>(new ActionBase[]{_singleChoiceAction}); }
        }

        #endregion

        #region ISupportUpdate Members

        private ISupportUpdate _tmpControl;

        public void BeginUpdate(){
            _tmpControl = _mainNavControl as ISupportUpdate;
            if (_tmpControl != null){
                _tmpControl.BeginUpdate();
            }
        }

        public void EndUpdate(){
            if (_tmpControl != null){
                _tmpControl.EndUpdate();
            }
            _tmpControl = null;
        }

        #endregion

        #region ITestable Members

        public string TestCaption{
            get { return _singleChoiceAction != null ? _singleChoiceAction.Caption : ""; }
        }

        public IJScriptTestControl TestControl{
            get { return null; }
        }

        public string ClientId{
            get { return _mainNavControl.TestControl.ClientID; }
        }

        public virtual TestControlType TestControlType{
            get { return TestControlType.Action; }
        }

        public event EventHandler<ControlInitializedEventArgs> ControlInitialized;

        #endregion

        #region ISupportCallbackStartupScriptRegistering Members

        public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;

        #endregion

        //#region ISupportNavigationActionContainerTesting Members
        //bool ISupportNavigationActionContainerTesting.IsItemControlVisible(ChoiceActionItem item)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemVisible(item);
        //}
        //int ISupportNavigationActionContainerTesting.GetGroupCount()
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetGroupCount();
        //}
        //string ISupportNavigationActionContainerTesting.GetGroupControlCaption(ChoiceActionItem groupItem)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetItemCaption(groupItem);
        //}
        //int ISupportNavigationActionContainerTesting.GetGroupChildControlCount(ChoiceActionItem groupItem)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetSubItemsCount(groupItem);
        //}
        //string ISupportNavigationActionContainerTesting.GetChildControlCaption(ChoiceActionItem item)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetItemCaption(item);
        //}
        //bool ISupportNavigationActionContainerTesting.GetChildControlEnabled(ChoiceActionItem item)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemEnabled(item);
        //}
        //bool ISupportNavigationActionContainerTesting.GetChildControlVisible(ChoiceActionItem item)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemVisible(item);
        //}
        //bool ISupportNavigationActionContainerTesting.IsGroupExpanded(ChoiceActionItem item)
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsGroupExpanded(item);
        //}
        //string ISupportNavigationActionContainerTesting.GetSelectedItemCaption()
        //{
        //    return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetSelectedItemCaption();
        //}
        //public Control NavigationControl
        //{
        //    get { return MainNavControl.Control; }
        //}
        //INavigationControlTestable ISupportNavigationActionContainerTesting.NavigationControl
        //{
        //    get { return (INavigationControlTestable)MainNavControl; }
        //}
        //#endregion

        //#region ITestableEx Members
        //public Type RegisterControlType
        //{
        //    get { return GetType(); }
        //}
        //#endregion
        //#region ISupportAdditionalParametersTestControl Members
        //public ICollection<string> GetAdditionalParameters(object navControl)
        //{
        //    ISupportAdditionalParametersTestControl additionalParametersTestControl = MainNavControl as ISupportAdditionalParametersTestControl;
        //    if (additionalParametersTestControl != null)
        //    {
        //        return additionalParametersTestControl.GetAdditionalParameters(additionalParametersTestControl);
        //    }
        //    return new string[0];
        //}
        //#endregion
    }
}