using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.Persistent.Base;

namespace DevExpress.ExpressApp.ConditionalEditorState.Win {
    /// <summary>
    /// A DetailView controller for the Windows Forms platform that provides the capability to customize the view's editors.
    /// </summary>
    public class EditorStateDetailViewController : EditorStateDetailViewControllerBase {
        protected Dictionary<string, BaseLayoutItem> itemsInfoHolder = null;
        protected WinLayoutManager layoutManager = null;
                                          
        protected override void OnActivated() {
            base.OnActivated();
            if (NeedsCustomization) {
                itemsInfoHolder = new Dictionary<string, BaseLayoutItem>();
                layoutManager = (WinLayoutManager)DetailView.LayoutManager;
                layoutManager.ItemCreated += OnLayoutManagerItemCreated;
            }
        }
        public new LayoutControl Control {
            get {
                return base.Control as LayoutControl;
            }
        }
        protected override void OnViewControlsCreated(object sender, EventArgs e) {
            base.OnViewControlsCreated(sender, e);
            if (IsReady) {
                Control.Paint += OnControlPaint;
                if (!Control.IsHandleCreated) {
                    Control.HandleCreated += OnControlHandleCreated;
                } else {
                    InvalidateRules(true);
                }
            }
        }
        protected override void ResourcesReleasing() {
            if (NeedsCustomization) {
                itemsInfoHolder = null;
                layoutManager.ItemCreated -= OnLayoutManagerItemCreated;
            }
            if (IsReady) {
                Control.Paint -= OnControlPaint;
                Control.HandleCreated -= OnControlHandleCreated;
            }
            base.ResourcesReleasing();
        }
        private void OnControlPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
            ForceCustomization();
        }
        private void OnControlHandleCreated(object sender, EventArgs e) {
            InvalidateRules(true);
        }
        protected override void ForceCustomizationCore(object currentObject, DevExpress.ExpressApp.ConditionalEditorState.Core.EditorStateRule rule) {
            try {
                Control.BeginUpdate();
                Control.BeginInit();
                base.ForceCustomizationCore(currentObject, rule);
            } finally {
                Control.EndInit();
                Control.EndUpdate();
            }
        }
        public override bool IsReady {
            get {
                return base.IsReady && (Control is LayoutControl);
            }
        }
        private void OnLayoutManagerItemCreated(object sender, ItemCreatedEventArgs e) {
            if (e.DetailViewItem is PropertyEditor) {
                string property = ((PropertyEditor)e.DetailViewItem).PropertyName;
                itemsInfoHolder.Add(property, e.Item);
            }
            RaiseCustomizeLayoutItem(e);
            OnCustomizeLayoutItem(e);
        }
        protected virtual void OnCustomizeLayoutItem(ItemCreatedEventArgs e) {}
        protected BaseLayoutItem GetLayoutItem(string property) {
            BaseLayoutItem layoutItem = null;
            try {
                layoutItem = itemsInfoHolder[property];
            } catch (KeyNotFoundException) {}
            return layoutItem;
        }
        protected override void HideShowEditor(string property, bool hidden) {
            BaseLayoutItem item = GetLayoutItem(property);
            if (item != null) {
                HideShowLayoutItemCore(item, hidden);
            } else {
                Tracing.Tracer.LogWarning(string.Format(EditorStateLocalizer.Active.GetLocalizedString("CannotFindInfoForProperty"), "BaseLayoutItem", property));
            }
        }
        public void HideShowLayoutItemCore(BaseLayoutItem item, bool hidden) {
            if (hidden == (item.Visibility == LayoutVisibility.Never)) {
                return;
            } else {
                item.ShowInCustomizationForm = !hidden;
                item.Visibility = hidden ? LayoutVisibility.Never : LayoutVisibility.Always;

                if (item.Parent != null) {
                    int count = GetVisibleItemsCount(item.Parent);
                    if (count > 0) {
                        item.Parent.ShowInCustomizationForm = true;
                        item.Parent.Visibility = LayoutVisibility.Always;
                    } else {
                        item.Parent.ShowInCustomizationForm = false;
                        item.Parent.Visibility = LayoutVisibility.Never;
                    }

                    if (item.Parent.ParentTabbedGroup != null) {
                        count = GetVisibleTabPagesCount(item.Parent.ParentTabbedGroup);
                        if (count > 0) {
                            item.Parent.ParentTabbedGroup.ShowInCustomizationForm = true;
                            item.Parent.ParentTabbedGroup.Visibility = LayoutVisibility.Always;

                            item.Parent.ParentTabbedGroup.SelectedTabPageIndex = 0;
                        } else {
                            item.Parent.ParentTabbedGroup.ShowInCustomizationForm = false;
                            item.Parent.ParentTabbedGroup.Visibility = LayoutVisibility.Never;
                        }
                    }
                }
            }
        }
        private int GetVisibleItemsCount(LayoutGroup layoutGroup) {
            int count = 0;
            foreach (BaseLayoutItem item in layoutGroup.Items) {
                if (item.Visibility == LayoutVisibility.Always) {
                    count++;
                }
            }
            return count;
        }
        private int GetVisibleTabPagesCount(TabbedGroup tabbedGroup) {
            int count = 0;
            foreach (LayoutGroup page in tabbedGroup.TabPages) {
                if (page.Visibility == LayoutVisibility.Always) {
                    count++;
                }
            }
            return count;
        }
        protected void RaiseCustomizeLayoutItem(ItemCreatedEventArgs args) {
            if (CustomizeLayoutItem != null) {
                CustomizeLayoutItem(this, args);
            }
        }
        /// <summary>
        /// Provides the capability to customize individual layout items.
        /// </summary>
        public event EventHandler<ItemCreatedEventArgs> CustomizeLayoutItem;
    }
}