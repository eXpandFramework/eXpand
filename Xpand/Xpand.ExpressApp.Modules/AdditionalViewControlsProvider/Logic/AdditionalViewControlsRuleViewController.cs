using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.Logic;
using Fasterflect;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController:ViewController {
        Dictionary<string, object> _infoToLayoutMapCore;
        LogicRuleViewController _logicRuleViewController;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute += OnLogicRuleExecute;
            Frame.Disposing+=FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= OnLogicRuleExecute;
        }

        protected bool HasRules {
            get { return LogicRuleManager.HasRules<AdditionalViewControlsLogicInstaller>(View.ObjectTypeInfo); }
        }
        
        void OnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var info = logicRuleExecuteEventArgs.LogicRuleInfo;
            var additionalViewControlsRule = info.Rule as IAdditionalViewControlsRule;
            if (Frame != null && additionalViewControlsRule!=null) {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                
                object viewSiteControl = GetContainerControl(viewSiteTemplate, additionalViewControlsRule);
                if (viewSiteControl != null) {
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, info.View.ObjectTypeInfo.Type);
                    Type controlType = calculator.ControlsRule.ControlType;
                    ICollection controls = GetControls(viewSiteControl);
                    IAdditionalViewControl additionalViewControl = FindControl(additionalViewControlsRule, controls);
                    if (info.Active && ViewContextIsCorrect(additionalViewControlsRule)) {

                        var control = GetControl(controlType, additionalViewControl, additionalViewControlsRule);
                        if (control != null) {
                            control.Rule = additionalViewControlsRule;
                            calculator.ControlsRule.DecoratorType.CreateInstance(new[] { info.View, (object)control, additionalViewControlsRule });
                            if (additionalViewControl == null) {
                                InitializeControl(control, additionalViewControlsRule, calculator, logicRuleExecuteEventArgs.ExecutionContext);
                                AddControl(control, controls, info);
                            }
                        }
                    } else if (additionalViewControl != null) {
                        controls.CallMethod("Remove",new[]{GetControlType()}, new object[]{additionalViewControl});
                    }
                }
            }

        }

        protected abstract Type GetControlType();

        protected Dictionary<string, object> RuleToLayoutMap {
            get { return _infoToLayoutMapCore ?? (_infoToLayoutMapCore = new Dictionary<string, object>()); }
        }
        
        protected void ResetInfoToLayoutMap() {
            RuleToLayoutMap.Clear();
        }
        protected void FillInfoToLayoutMap(ViewItem detailViewItem, IModelViewLayoutElement itemModel, object layoutItem) {
            var item = detailViewItem as AdditionalViewControlsItem;
            if (item != null) {
                var id = item.Model.Rule.Id;
                if (RuleToLayoutMap.ContainsKey(id))
                    RuleToLayoutMap[id] = layoutItem;
                else
                    RuleToLayoutMap.Add(id, layoutItem);
            }
        }
                
        bool ViewContextIsCorrect(IAdditionalViewControlsRule rule) {
            return rule.Position != Position.DetailViewItem || !(View is ListView);
        }
        
        protected virtual IAdditionalViewControl GetControl(Type controlType, IAdditionalViewControl additionalViewControl, IAdditionalViewControlsRule rule) {
            var control = additionalViewControl ?? controlType.CreateInstance() as IAdditionalViewControl;
            var manager = control as ISupportLayoutManager;
            if (manager != null) {
                if (rule.Position != Position.DetailViewItem)
                    throw new ArgumentException("Rule with Id:" + rule.Id + " position should be set to " + Position.DetailViewItem);
                if (RuleToLayoutMap.ContainsKey(rule.Id))
                    manager.LayoutItem = RuleToLayoutMap[rule.Id];
            }
            return control;
        }
        
        protected object GetContainerControl(IViewSiteTemplate viewSiteTemplate, IAdditionalViewControlsRule rule) {
            if (rule.Position == Position.DetailViewItem && View is DetailView) {
                var modelAdditionalViewControlsItem = ((DetailView)View).Items.OfType<AdditionalViewControlsItem>().FirstOrDefault(item => item.Model.Rule.Id == rule.Id);
                return modelAdditionalViewControlsItem != null ? modelAdditionalViewControlsItem.Control : null;
            }
            return viewSiteTemplate.ViewSiteControl;
        }
        
        ICollection GetControls(object viewSiteControl) {
            return (ICollection)(viewSiteControl.GetPropertyValue("Controls"));
        }
        
        IAdditionalViewControl FindControl(IAdditionalViewControlsRule rule, ICollection controls) {
            return controls.OfType<IAdditionalViewControl>().FirstOrDefault(o => o.Rule.Id == rule.Id);
        }
        
        protected virtual void RemoveControl(IList controls, object firstOrDefault, IAdditionalViewControlsRule rule) {
            if (rule.Position != Position.DetailViewItem)
                controls.Remove(firstOrDefault);
        }
        
        protected virtual void AddControl(object control, object controls, LogicRuleInfo info) {
            controls.CallMethod("Add",new[]{GetControlType()}, new[]{control});
        }
        
        protected virtual void InitializeControl(object control, IAdditionalViewControlsRule additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context) {
            var appeareance = control as ISupportAppeareance;
            if (appeareance != null) {
                var supportAppeareance = appeareance;
                supportAppeareance.BackColor = additionalViewControlsRule.BackColor;
                supportAppeareance.Height = additionalViewControlsRule.Height;
                supportAppeareance.ForeColor = additionalViewControlsRule.ForeColor;
                supportAppeareance.FontStyle = additionalViewControlsRule.FontStyle;
                supportAppeareance.FontSize = additionalViewControlsRule.FontSize;
                supportAppeareance.ImageName = additionalViewControlsRule.ImageName;
            }
        }
    }
}
