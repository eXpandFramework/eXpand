using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
using MenuItem = DevExpress.Web.MenuItem;
using MenuItemCollection = DevExpress.Web.MenuItemCollection;

namespace Xpand.ExpressApp.Web.Templates{
    public class CreateCustomGroupControlEventArgs : EventArgs{
        public CreateCustomGroupControlEventArgs(ChoiceActionItem groupItem){
            GroupItem = groupItem;
        }

        public ChoiceActionItem GroupItem { get; private set; }

        public Control Control { get; set; }
    }

    public class ASPxMenuGroupChoiceActionItem : ChoiceActionItemWrapper{
        private readonly MenuItem _currentMenuGroup;

        public ASPxMenuGroupChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item)
            : base(item, action){
            _currentMenuGroup = new MenuItem();
            if (action.Items.IndexOf(item) == 0){
                //CurrentMenuGroup.HeaderStyle.CssClass += " FirstHeader";
                //CurrentMenuGroup.HeaderStyleCollapsed.CssClass = " FirstHeaderCollapsed";
            }
            if (action.Items.IndexOf(item) == action.Items.Count - 1){
                //CurrentMenuGroup.HeaderStyle.CssClass = " LastHeader";
                //CurrentMenuGroup.HeaderStyleCollapsed.CssClass = " LastHeaderCollapsed";
            }
            //CurrentMenuGroup.Expanded = false;
            SyncronizeWithItem();
        }

        public MenuItem MenuGroup{
            get { return _currentMenuGroup; }
        }

        public override void SetImageName(string imageName){
            ASPxImageHelper.SetImageProperties(_currentMenuGroup.Image, imageName);
        }

        public override void SetCaption(string caption){
            _currentMenuGroup.Text = caption;
            _currentMenuGroup.Name = caption;
        }

        public override void SetData(object data){
        }

        public override void SetShortcut(string shortcutString){
        }

        public override void SetEnabled(bool enabled){
        }

        public override void SetVisible(bool visible){
            _currentMenuGroup.Visible = visible;
        }

        public override void SetToolTip(string toolTip){
            _currentMenuGroup.ToolTip = toolTip;
        }
    }

    public class ASPxMenuItemChoiceActionItem : ChoiceActionItemWrapper{
        private readonly SingleChoiceAction _currentAction;

        public ASPxMenuItemChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item)
            : base(item, action){
            _currentAction = action;
            CurrentMenuItem = new MenuItem{Name = item.GetIdPath()};
            SyncronizeWithItem();
        }

        public MenuItem CurrentMenuItem { get; private set; }

        public override void SetImageName(string imageName){
            ASPxImageHelper.SetImageProperties(CurrentMenuItem.Image, imageName);
        }

        public override void SetCaption(string caption){
            CurrentMenuItem.Text = caption;
        }

        public override void SetData(object data){
        }

        public override void SetShortcut(string shortcutString){
        }

        public override void SetEnabled(bool enabled){
            CurrentMenuItem.Enabled = enabled;
            CurrentMenuItem.ClientEnabled = enabled;
        }

        public override void SetVisible(bool visible){
            CurrentMenuItem.Visible = visible;
        }

        public override void SetToolTip(string toolTip){
            CurrentMenuItem.ToolTip = toolTip;
        }

        public void ExecuteAction(){
            if (_currentAction.Active && _currentAction.Enabled){
                _currentAction.DoExecute(ActionItem);
            }
        }
    }

    internal class ASPxMenuCustomControlItem : MenuItem, IDisposable{
        private readonly Control _control;

        public ASPxMenuCustomControlItem(Control control){
            _control = control;
        }

        internal void InitTemplate(){
            Template = new ASPxMenuCustomGroupControlTemplate(_control);
        }

        #region IDisposable Members

        public void Dispose(){
            if (_control != null){
                ((IDisposable) _control).Dispose();
            }
        }

        #endregion

        internal class ASPxMenuCustomGroupControlTemplate : ITemplate, IDisposable{
            private readonly Control _control;

            public ASPxMenuCustomGroupControlTemplate(Control control){
                _control = control;
            }

            #region ITemplate Members

            public void InstantiateIn(Control container){
                container.Controls.Add(_control);
            }

            #endregion

            #region IDisposable Members

            public void Dispose(){
                if (_control != null){
                    ((IDisposable) _control).Dispose();
                }
            }

            #endregion
        }
    }

    public class ASPxMenuNavigationControl : IWebNavigationControl, INavigationControlTestable, IDisposable,
        ISupportCallbackStartupScriptRegistering, IXafCallbackHandler /*, ISupportAdditionalParametersTestControl*/{
        private readonly ASPxMenu _asPxMenuControl;
        private readonly LightDictionary<ChoiceActionItem, MenuItem> _actionItemToMenuGroupMap;
        private readonly Dictionary<ChoiceActionItem, MenuItem> _actionItemToMenuItemMap;
        private readonly List<ASPxMenuGroupChoiceActionItem> _groupWrappers;
        private readonly Dictionary<MenuItem, ASPxMenuItemChoiceActionItem> _menuItemToWrapperMap;
        private ChoiceActionItemCollection _actionItems;
        internal SingleChoiceAction SingleChoiceAction;

        public ASPxMenuNavigationControl(){
            _asPxMenuControl = RenderHelper.CreateASPxMenu();
            _asPxMenuControl.AllowSelectItem = true;
            _asPxMenuControl.Border.BorderStyle = BorderStyle.None;
            _asPxMenuControl.ID = "NB";
            _asPxMenuControl.ItemClick += ASPxMenuControl_ItemClick;
            _asPxMenuControl.Load += ASPxMenuControl_Load;
            _actionItemToMenuItemMap = new Dictionary<ChoiceActionItem, MenuItem>();
            _actionItemToMenuGroupMap = new LightDictionary<ChoiceActionItem, MenuItem>();
            _menuItemToWrapperMap = new Dictionary<MenuItem, ASPxMenuItemChoiceActionItem>();
            _groupWrappers = new List<ASPxMenuGroupChoiceActionItem>();
        }

        private void BuildControl(){
            _actionItemToMenuItemMap.Clear();
            _actionItemToMenuGroupMap.Clear();
            ClearItemWrappers();
            _asPxMenuControl.Items.Clear();
            if (_actionItems.Count > 0){
                FillMenuContents(_asPxMenuControl.Items, _actionItems);
            }
            UpdateSelection();
        }


        private void FillMenuContents(MenuItemCollection holderMenuItems, ChoiceActionItemCollection actionItems){
            foreach (ChoiceActionItem groupValue in actionItems){
                if (!groupValue.Active){
                    continue;
                }

                if (groupValue.Items.Count == 0){
                    var itemWrapper = new ASPxMenuItemChoiceActionItem(SingleChoiceAction,
                        groupValue);
                    _menuItemToWrapperMap.Add(itemWrapper.CurrentMenuItem, itemWrapper);
                    holderMenuItems.Add(itemWrapper.CurrentMenuItem);
                    _actionItemToMenuItemMap.Add(groupValue, itemWrapper.CurrentMenuItem);
                }
                else{
                    var groupItem = new ASPxMenuGroupChoiceActionItem(SingleChoiceAction,
                        groupValue);
                    _groupWrappers.Add(groupItem);
                    MenuItem group = groupItem.MenuGroup;
                    _actionItemToMenuGroupMap.Add(groupValue, group);
                    holderMenuItems.Add(group);
                    var itemsDisplayStyle = ItemsDisplayStyle.LargeIcons;
                    if (groupValue.Model != null){
                        itemsDisplayStyle = ItemsDisplayStyle.List;
                        var style = groupValue.Model as IModelChoiceActionItemChildItemsDisplayStyle;
                        if (style != null){
                            itemsDisplayStyle =
                                style.ChildItemsDisplayStyle;
                        }
                    }
                    var args = new CreateCustomGroupControlEventArgs(groupValue);
                    OnCreateCustomGroupControl(args);
                    if (args.Control != null){
                        var customControlItem = new ASPxMenuCustomControlItem(args.Control);
                        customControlItem.InitTemplate();
                        group.Items.Add(customControlItem);
                    }
                    else{
                        switch (itemsDisplayStyle){
                            case ItemsDisplayStyle.LargeIcons:
                            case ItemsDisplayStyle.List:
                                if (groupValue.IsHierarchical()){
                                    FillMenuContents(group.Items, groupValue.Items);
                                }
                                else{
                                    FillMenuGroup(group, groupValue);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void FillMenuGroup(MenuItem group, ChoiceActionItem groupValue){
            foreach (ChoiceActionItem itemValue in groupValue.Items){
                if (!itemValue.Active){
                    continue;
                }
                var itemWrapper = new ASPxMenuItemChoiceActionItem(SingleChoiceAction, itemValue);
                _menuItemToWrapperMap.Add(itemWrapper.CurrentMenuItem, itemWrapper);
                group.Items.Add(itemWrapper.CurrentMenuItem);
                _actionItemToMenuItemMap.Add(itemValue, itemWrapper.CurrentMenuItem);
            }
        }


        private void singleChoiceAction_ItemsChanged(object sender, ItemsChangedEventArgs e){
            BuildControl();
        }

        private void UpdateSelection(){
            _asPxMenuControl.SelectedItem = null;
            if (SingleChoiceAction != null && SingleChoiceAction.SelectedItem != null){
                ChoiceActionItem actionItem = SingleChoiceAction.SelectedItem;
                if (_actionItemToMenuItemMap.ContainsKey(actionItem)){
                    MenuItem itemLink = _actionItemToMenuItemMap[actionItem];
                    _asPxMenuControl.SelectedItem = itemLink;
                }
            }
        }

        private void action_SelectedItemChanged(object sender, EventArgs e){
            UpdateSelection();
        }

        private void SubscribeToAction(){
            SingleChoiceAction.ItemsChanged += singleChoiceAction_ItemsChanged;
            SingleChoiceAction.SelectedItemChanged += action_SelectedItemChanged;
        }

        private void UnsubscribeFromAction(){
            if (SingleChoiceAction != null){
                SingleChoiceAction.SelectedItemChanged -= action_SelectedItemChanged;
                SingleChoiceAction.ItemsChanged -= singleChoiceAction_ItemsChanged;
            }
        }

        private void UnsubscribeAll(){
            UnsubscribeFromAction();
            SingleChoiceAction = null;
            _actionItemToMenuItemMap.Clear();
            _actionItemToMenuGroupMap.Clear();
        }

        private MenuItem FindMenuGroupControl(ChoiceActionItem item){
            if (item != null && _actionItemToMenuGroupMap.ContainsKey(item)){
                return _actionItemToMenuGroupMap[item];
            }
            return null;
        }

        private void ClearItemWrappers(){
            foreach (ASPxMenuItemChoiceActionItem itemWrapper in _menuItemToWrapperMap.Values){
                itemWrapper.Dispose();
            }
            _menuItemToWrapperMap.Clear();
            foreach (ASPxMenuGroupChoiceActionItem groupWrapper in _groupWrappers){
                groupWrapper.Dispose();
            }
            _groupWrappers.Clear();
        }

        protected virtual void OnCreateCustomGroupControl(CreateCustomGroupControlEventArgs args){
            if (CreateCustomGroupControl != null){
                CreateCustomGroupControl(this, args);
            }
        }

        protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e){
            if (RegisterCallbackStartupScript != null){
                RegisterCallbackStartupScript(this, e);
            }
        }


        private void ASPxMenuControl_ItemClick(object source, MenuItemEventArgs e){
            if (e.Item != null && _menuItemToWrapperMap.ContainsKey(e.Item)){
                _menuItemToWrapperMap[e.Item].ExecuteAction();
            }
        }

        private void ASPxMenuControl_Load(object sender, EventArgs e){
            var holder = _asPxMenuControl.Page as ICallbackManagerHolder;
            if (holder != null){
                holder.CallbackManager.RegisterHandler(_asPxMenuControl, this);
                _asPxMenuControl.ClientSideEvents.ItemClick = @"function(s,e) {
					e.processOnServer = false;" +
                                                             holder.CallbackManager.GetScript(_asPxMenuControl.UniqueID,
                                                                 "e.item.name", String.Empty,
                                                                 SingleChoiceAction.Model.GetValue<bool>(
                                                                     "IsPostBackRequired")) +
                                                             @"}";
            }
        }

        public event EventHandler<CreateCustomGroupControlEventArgs> CreateCustomGroupControl;

        #region INavigationControl

        public void SetNavigationActionItems(ChoiceActionItemCollection actionItems, SingleChoiceAction action){
            Guard.ArgumentNotNull(action, "action");
            UnsubscribeFromAction();
            _actionItems = actionItems;
            SingleChoiceAction = action;
            BuildControl();
            SubscribeToAction();
        }

        public Control Control{
            get { return _asPxMenuControl; }
        }

        public Control TestControl{
            get { return _asPxMenuControl; }
        }

        #endregion

        #region ISupportNavigationActionContainerTesting

        public bool IsItemControlVisible(ChoiceActionItem item){
            bool result = false;
            if (_actionItemToMenuGroupMap[item] != null){
                result = _actionItemToMenuGroupMap[item].Visible;
            }
            return result;
        }

        public int GetGroupCount(){
            return _asPxMenuControl.Items.Count;
        }

        public string GetGroupControlCaption(ChoiceActionItem item){
            if (_actionItemToMenuGroupMap[item] != null){
                return _actionItemToMenuGroupMap[item].Text;
            }
            throw new ArgumentOutOfRangeException();
        }

        public int GetGroupChildControlCount(ChoiceActionItem item){
            if (_actionItemToMenuGroupMap[item] != null){
                return _actionItemToMenuGroupMap[item].Items.Count;
            }
            throw new ArgumentOutOfRangeException();
        }

        public string GetChildControlCaption(ChoiceActionItem item){
            if (_actionItemToMenuItemMap[item] != null){
                return _actionItemToMenuItemMap[item].Text;
            }
            throw new ArgumentOutOfRangeException();
        }

        public bool GetChildControlEnabled(ChoiceActionItem item){
            if (_actionItemToMenuItemMap[item] != null){
                return _actionItemToMenuItemMap[item].ClientEnabled;
            }
            throw new ArgumentOutOfRangeException();
        }

        public bool GetChildControlVisible(ChoiceActionItem item){
            if (_actionItemToMenuItemMap[item] != null){
                return _actionItemToMenuItemMap[item].Visible;
            }
            throw new ArgumentOutOfRangeException();
        }

        public bool IsGroupExpanded(ChoiceActionItem item){
            if (_actionItemToMenuGroupMap[item] != null){
                return false;
            }
            throw new ArgumentOutOfRangeException();
        }

        public string GetSelectedItemCaption(){
            if (_asPxMenuControl.SelectedItem != null){
                return _asPxMenuControl.SelectedItem.Text;
            }
            return string.Empty;
        }

        #endregion

        #region ISupportNavigationActionContainerTesting

        #endregion

        #region INavigationControlTestable Members

        bool INavigationControlTestable.IsItemEnabled(ChoiceActionItem item){
            if (_actionItemToMenuItemMap.ContainsKey(item)){
                return _actionItemToMenuItemMap[item].Enabled;
            }

            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                return _actionItemToMenuGroupMap[item].Visible;
            }
            return false;
        }

        bool INavigationControlTestable.IsItemVisible(ChoiceActionItem item){
            if (_actionItemToMenuItemMap.ContainsKey(item)){
                return _actionItemToMenuItemMap[item].Visible;
            }

            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                return _actionItemToMenuGroupMap[item].Visible;
            }
            return false;
        }

        int INavigationControlTestable.GetSubItemsCount(ChoiceActionItem item){
            if (_actionItemToMenuItemMap.ContainsKey(item)){
                return 0;
            }

            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                MenuItem menuGroup = _actionItemToMenuGroupMap[item];
                if (menuGroup.Items.Count > 0){
                    return menuGroup.Items[0].Items.Count;
                }
                return _actionItemToMenuGroupMap[item].Items.Count;
            }
            return 0;
        }

        string INavigationControlTestable.GetItemCaption(ChoiceActionItem item){
            if (_actionItemToMenuItemMap.ContainsKey(item)){
                return _actionItemToMenuItemMap[item].Text;
            }

            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                return _actionItemToMenuGroupMap[item].Text;
            }
            return string.Empty;
        }

        string INavigationControlTestable.GetItemToolTip(ChoiceActionItem item){
            if (_actionItemToMenuItemMap.ContainsKey(item)){
                return _actionItemToMenuItemMap[item].ToolTip;
            }
            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                return _actionItemToMenuGroupMap[item].ToolTip;
            }
            return string.Empty;
        }

        int INavigationControlTestable.GetGroupCount(){
            return _asPxMenuControl.Items.Count;
        }

        int INavigationControlTestable.GetSubGroupCount(ChoiceActionItem item){
            MenuItem group = FindMenuGroupControl(item);

            if (group != null && group.Items.Count > 0){
                return group.Items[0].Items.Count;
            }
            return 0;
        }

        bool INavigationControlTestable.IsGroupExpanded(ChoiceActionItem item){
            if (_actionItemToMenuGroupMap.ContainsKey(item)){
                return false;
            }
            return false;
        }

        string INavigationControlTestable.GetSelectedItemCaption(){
            if (_asPxMenuControl.SelectedItem != null){
                return _asPxMenuControl.SelectedItem.Text;
            }

            return string.Empty;
        }

        #endregion

        #region IDisposable Members

        public void Dispose(){
            _asPxMenuControl.ItemClick -= ASPxMenuControl_ItemClick;
            _asPxMenuControl.Load -= ASPxMenuControl_Load;
            ClearItemWrappers();
            UnsubscribeAll();
            foreach (MenuItem group in _asPxMenuControl.Items){
                foreach (MenuItem item in group.Items){
                    var disposable = item as IDisposable;
                    if (disposable != null){
                        disposable.Dispose();
                    }
                }
                var @groupDispoable = @group as IDisposable;
                if (@groupDispoable != null){
                    @groupDispoable.Dispose();
                }
            }
            RegisterCallbackStartupScript = null;
        }

        #endregion

        #region ISupportCallbackStartupScriptRegistering Members

        public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;

        #endregion

        #region IXafCallbackHandler Members

        public void ProcessAction(string parameter){
            if (SingleChoiceAction.Active && SingleChoiceAction.Enabled){
                ChoiceActionItem item = SingleChoiceAction.FindItemByIdPath(parameter);
                if (item != null){
                    SingleChoiceAction.DoExecute(item);
                }
            }
        }

        #endregion
    }
}