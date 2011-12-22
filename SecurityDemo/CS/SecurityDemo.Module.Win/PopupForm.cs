using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Controls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.XtraLayout;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win;

namespace FeatureCenter.Module.Win {
    public partial class PopupForm : DevExpress.XtraEditors.XtraForm, IWindowTemplate, ISupportViewChanged, ISupportStoreSettings, IViewSiteTemplate, IBarManagerHolder, DevExpress.ExpressApp.Demos.IHintTemplate {
		private const int viewControlPreferredWidth = 350;
		private const int viewControlPreferredHeight = 215;
		private const int minWidth = 420;
		private const int minHeight = 150;
		private const string FrameTemplatesPopupForm = @"FrameTemplates\PopupForm";
		private bool autoShrink;
		private bool ignoreMinimumSize;
		private DevExpress.ExpressApp.View view;
        private TemplatesHelper locaizationHelper;
		private void MoveFocusToFirstViewControl() {
			if(viewSitePanel != null) {
				viewSitePanel.SelectNextControl(viewSitePanel, true, true, true, false);
			}
		}
		private Size GetViewSitePanelMinSize() {
			Control viewControl = viewSitePanel.Controls[0];
			if(viewControl is LayoutControl) {
				LayoutControl layoutControl = viewControl as LayoutControl;
				layoutControl.BeginUpdate();
				layoutControl.EndUpdate();
			}
			IXtraResizableControl resizableControl = viewControl as IXtraResizableControl;
			if(resizableControl != null) {
				return resizableControl.MinSize;
			}
			return viewControl.MinimumSize;
		}
		public void UpdateSize() {
			bottomPanel.MinimumSize = bottomPanel.Root.MinSize;
			if(viewSitePanel.Controls.Count > 0) {
				Size viewControlMinimumSize = GetViewSitePanelMinSize();
				if(CustomGetMinSize != null) {
					CustomSizeEventArgs eventArgs = new CustomSizeEventArgs(viewControlMinimumSize);
					CustomGetMinSize(this, eventArgs);
					if(eventArgs.Handled) {
						viewControlMinimumSize = eventArgs.CustomSize;
					}
				}
				
				int nonClientWidth = Size.Width - ClientSize.Width;
				int nonClientHeight = Size.Height - ClientSize.Height;

				int viewControlMinWidthWithPaddings = viewSitePanel.Padding.Left + viewSitePanel.Padding.Right + viewControlMinimumSize.Width;
				int viewControlMinHeightWithPaddings = viewSitePanel.Padding.Top + viewSitePanel.Padding.Bottom + viewControlMinimumSize.Height;

				Size calculatedClientSize;
				if(WindowState == FormWindowState.Maximized) {
					calculatedClientSize = new Size(
						Math.Max(ClientSize.Width + viewControlMinWidthWithPaddings - viewSitePanel.Width, ClientSize.Width),
						Math.Max(ClientSize.Height + viewControlMinHeightWithPaddings - viewSitePanel.Height, ClientSize.Height));
				}
				else {
					calculatedClientSize = new Size(
						ClientSize.Width + viewControlMinWidthWithPaddings - viewSitePanel.Width,
						ClientSize.Height + viewControlMinHeightWithPaddings - viewSitePanel.Height);
				}
				if(CustomizeClientSize != null) {
					CustomSizeEventArgs eventArgs = new CustomSizeEventArgs(calculatedClientSize);
					CustomizeClientSize(this, eventArgs);
					if(eventArgs.Handled) {
						calculatedClientSize = eventArgs.CustomSize;
					}
				}
				ClientSize = calculatedClientSize;

				if(!ignoreMinimumSize) {
					Size calculatedMinumumSize = new Size(
						Math.Max(nonClientWidth + ClientSize.Width + viewControlMinWidthWithPaddings - viewSitePanel.Width, minWidth),
						Math.Max(nonClientHeight + ClientSize.Height + viewControlMinHeightWithPaddings - viewSitePanel.Height, minHeight));
					if(CustomizeMinimumSize != null) {
						CustomSizeEventArgs eventArgs = new CustomSizeEventArgs(calculatedMinumumSize);
						CustomizeMinimumSize(this, eventArgs);
						if(eventArgs.Handled) {
							calculatedMinumumSize = eventArgs.CustomSize;
						}
					}
					MinimumSize = calculatedMinumumSize;
				}

			}
		}
        protected virtual void OnBarMangerChanged() {
            if(BarManagerChanged != null) {
                BarManagerChanged(this, EventArgs.Empty);
            }
        }
        protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(autoShrink) {
				UpdateSize();
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible && autoShrink) {
				UpdateSize();
			}
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			MoveFocusToFirstViewControl();
		}
		public PopupForm() {
			InitializeComponent();
            CustomizeClientSize += new System.EventHandler<DevExpress.ExpressApp.Win.CustomSizeEventArgs>(PopupForm_CustomizeClientSize);
            NativeMethods.SetExecutingApplicationIcon(this);
			autoShrink = true;
			ignoreMinimumSize = false;
			ShowInTaskbar = true;
			KeyPreview = true;

			ClientSize = new Size(
				ClientSize.Width - viewSitePanel.Width + viewSitePanel.Padding.Left + viewSitePanel.Padding.Right + viewControlPreferredWidth,
				ClientSize.Height - viewSitePanel.Height + viewSitePanel.Padding.Top + viewSitePanel.Padding.Bottom + viewControlPreferredHeight);
    
        }

        private void PopupForm_CustomizeClientSize(object sender, CustomSizeEventArgs e) {
            e.CustomSize = new System.Drawing.Size(e.CustomSize.Width, e.CustomSize.Height + hintPanel.Height);
            e.Handled = true;
        }
        private void hintPanel_SizeChanged(object sender, EventArgs e) {
            UpdateSize();
        }
		public void AddControl(Control control, string caption) {
			if(control != null) {
				viewSitePanel.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				Text = caption;
			}
		}
		public virtual ICollection<IActionContainer> GetContainers() {
            return actionContainersManager1.GetContainers();
		}
		public virtual void SetView(DevExpress.ExpressApp.View view) {
			this.view = view;
			viewSitePanel.Controls.Clear();
			if(view != null) {
				view.CreateControls();
				Control viewControl = (Control)view.Control;
				if(viewControl != null) {
					viewControl.Dock = DockStyle.Fill;
					viewSitePanel.SuspendLayout();
					try {
						if(viewControl is ISupportUpdate) {
							((ISupportUpdate)viewControl).BeginUpdate();
						}
						viewSitePanel.Controls.Add(viewControl);
						if(viewControl is ISupportUpdate) {
							((ISupportUpdate)viewControl).EndUpdate();
						}
					}
					finally {
						viewSitePanel.ResumeLayout();
					}
				}
                if(ViewChanged != null) {
                    ViewChanged(this, new TemplateViewChangedEventArgs(view));
                }
				if(view.Model != null) {
                    string imageName = ViewImageNameHelper.GetImageName(view);
					NativeMethods.SetFormIcon(this,
                        ImageLoader.Instance.GetImageInfo(imageName).Image,
                        ImageLoader.Instance.GetLargeImageInfo(imageName).Image);
				}
				else {
					NativeMethods.SetFormIcon(this,
						NativeMethods.ExeIconSmall,
						NativeMethods.ExeIconLarge);
				}
				Text = view.Caption;
				MoveFocusToFirstViewControl();
			}
		}
        private IModelFormState GetFormStateNode() {
            string viewId;
            if(view != null) {
                viewId = view.Id;
            }
            else {
                viewId = "Default";
            }
            IModelFormState result = modelTemplate.FormStates[viewId];
            if(result == null) {
                result = modelTemplate.FormStates.AddNode<IModelFormState>(viewId);
            }
            return result;
        }
        private IModelTemplateWin modelTemplate;
        public virtual void SetSettings(IModelTemplate modelTemplate) {
            this.modelTemplate = (IModelTemplateWin)modelTemplate;
            locaizationHelper = new TemplatesHelper(this.modelTemplate);
            formStateModelSynchronizer.Model = GetFormStateNode();
			autoShrink = autoShrink && !IsSizeable;
			ReloadSettings();
		}
		public virtual void ReloadSettings() {
            if(modelTemplate != null && IsSizeable) {
                formStateModelSynchronizer.ApplyModel(); 
			}
		}
		public virtual void SaveSettings() {
            if(modelTemplate != null && IsSizeable) {
                formStateModelSynchronizer.SynchronizeModel(); 
            }
		}
		public virtual void SetStatus(string[] messages) { }
		public virtual void SetStatus(System.Collections.Generic.ICollection<string> statusMessages) { }
		public virtual void SetCaption(string caption) {
            Text = caption;
        }
		public IActionContainer DefaultContainer {
			get { return actionContainersManager1.DefaultContainer; }
		}
		public virtual bool IsSizeable {
			get {
				return FormBorderStyle == FormBorderStyle.Sizable;
			}
			set {
				Rectangle storedBounds = DesktopBounds;
				if(value) {
					FormBorderStyle = FormBorderStyle.Sizable;
				}
				else {
					FormBorderStyle = FormBorderStyle.FixedDialog;
				}
				DesktopBounds = storedBounds;
				MinimizeBox = value;
				MaximizeBox = value;
			}
		}
		public ButtonsContainer ButtonsContainer {
			get { return buttonsContainer1; }
		}
		public bool AutoShrink {
			get { return autoShrink; }
			set { autoShrink = value; }
		}
		public object ViewSiteControl {
			get { return viewSitePanel; }
		}
		public event EventHandler<CustomSizeEventArgs> CustomizeClientSize;
		public event EventHandler<CustomSizeEventArgs> CustomizeMinimumSize;
        public event EventHandler BarManagerChanged;
        #region Obsolete 8.3
		[Obsolete("Use the 'CustomizeMinimumSize' event instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IgnoreMinimumSize {
			get { return ignoreMinimumSize; }
			set { ignoreMinimumSize = value; }
		}
		[Obsolete("Use the 'CustomizeMinimumSize' event instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<CustomSizeEventArgs> CustomGetMinSize;
		#endregion

		#region IBarManagerHolder Members

		public DevExpress.XtraBars.BarManager BarManager {
			get { return xafBarManager1; }
		}

		#endregion

        #region ISupportViewChanged Members

        public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;

        #endregion

        #region IHintTemplate Members

        public string Hint {
            get {
                return hintPanel.Text;
            }
            set {
                hintPanel.Text = value;
                hintPanel.Visible = !string.IsNullOrEmpty(value);
            }
        }

        #endregion
    }
    [System.ComponentModel.DisplayName("FeatureCenter PopupForm Template")]
    public class FeatureCenterPopupFormTemplateLocalizer : FrameTemplateLocalizer<PopupForm> { }
}
