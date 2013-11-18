using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars;

namespace Xpand.ExpressApp.Win.SystemModule.ToolTip {
    [ModelAbstractClass]
    public interface IModelClassNewObjectActionTooltip:IModelClass {
        [Category("eXpand")]
        [Localizable(true)]
        string NewObjectActionTooltip { get; set; }
    }
    public class NewObjectActionTooltipController:WindowController,IModelExtender {
        ToolTipHelper _helper;

        public NewObjectActionTooltipController() {
            TargetWindowType=WindowType.Main;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
            Frame.Disposing+=FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            Frame.TemplateChanged-=FrameOnTemplateChanged;
            if (_helper != null) {
                _helper.ShowToolTipInMenuItems = false;
            }
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var barManagerHolder = Frame.Template as IBarManagerHolder;
            if (_helper != null) {
                _helper.ShowToolTipInMenuItems = false;
            }
            if (barManagerHolder != null) {
                var newObjectAction = Frame.GetController<NewObjectViewController>().NewObjectAction;
                _helper = new ToolTipHelper(barManagerHolder.BarManager, newObjectAction) { ShowToolTipInMenuItems = true };
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass,IModelClassNewObjectActionTooltip>();
        }
    }

    public class ToolTipHelper {
        readonly BarManager _barManager;
        readonly SingleChoiceAction _newObjectAction;
        BarItemLink _TooltipItem;

        public ToolTipHelper(BarManager barManager, SingleChoiceAction newObjectAction) {
            _barManager = barManager;
            _newObjectAction = newObjectAction;
        }

        public bool ShowToolTipInMenuItems {
            set {
                _barManager.HighlightedLinkChanged -= OnHighlightedLinkChanged;
                if (value)
                    _barManager.HighlightedLinkChanged += OnHighlightedLinkChanged;
            }
        }


        BarItemLink TooltipItem {
            get { return _TooltipItem; }
            set {
                _TooltipItem = value;
                OnToolTipItemChanged();
            }
        }

        void OnHighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
            if (e.Link != null && e.Link.IsLinkInMenu)
                TooltipItem = e.Link;
            else
                TooltipItem = null;
        }

        void OnToolTipItemChanged() {
            ToolTipController.DefaultController.HideHint();
            if (TooltipItem != null)
                ShowItemSuperTip(TooltipItem);
        }

        void ShowItemSuperTip(BarItemLink item) {
            var args = new ToolTipControllerShowEventArgs{
                ToolTipType = ToolTipType.SuperTip,
                SuperTip = new SuperToolTip()
            };
            foreach (var choiceActionItem in _newObjectAction.Items) {
                var formatTestAction = EasyTestTagHelper.FormatTestAction(_newObjectAction.Caption + '.' + choiceActionItem.GetItemPath());
                if (formatTestAction==item.Item.Tag as string) {
                    var data = choiceActionItem.Data;
                    if (data!=null) {
                        var newObjectActionTooltip =
                            ((IModelClassNewObjectActionTooltip)
                             _newObjectAction.Application.Model.BOModel.GetClass((Type) data)).NewObjectActionTooltip;
                        if (!string.IsNullOrEmpty(newObjectActionTooltip)) {
                            args.SuperTip.Items.Add(newObjectActionTooltip);
                            ToolTipController.DefaultController.ShowHint(args);
                        }
                    }
                }
            }
            
        }
    }
}
