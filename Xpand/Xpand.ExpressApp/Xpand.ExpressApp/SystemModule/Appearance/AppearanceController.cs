using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Fastreflect;

namespace Xpand.ExpressApp.SystemModule.Appearance {
    public class AppearanceController : ViewController, IModelExtender {
        private DevExpress.ExpressApp.ConditionalAppearance.AppearanceController _appearanceController;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _appearanceController.CustomApplyAppearance -= AppearanceControllerOnCustomApplyAppearance;
            _appearanceController.CustomCreateAppearanceRule -= AppearanceControllerOnCustomCreateAppearanceRule;
            _appearanceController.CustomGetIsRulePropertiesEmpty -= AppearanceControllerOnCustomGetIsRulePropertiesEmpty;
        }

        protected override void OnActivated() {
            base.OnActivated();
            _appearanceController = Frame.GetController<DevExpress.ExpressApp.ConditionalAppearance.AppearanceController>();
            _appearanceController.CustomApplyAppearance += AppearanceControllerOnCustomApplyAppearance;
            _appearanceController.CustomCreateAppearanceRule += AppearanceControllerOnCustomCreateAppearanceRule;
            _appearanceController.CustomGetIsRulePropertiesEmpty += AppearanceControllerOnCustomGetIsRulePropertiesEmpty;
        }

        private void AppearanceControllerOnCustomApplyAppearance(object sender, ApplyAppearanceEventArgs e) {
            var appearanceRules =(List<DevExpress.ExpressApp.ConditionalAppearance.AppearanceRule>)
                    _appearanceController.CallMethod("GetAppearanceRules",new[]{typeof (ObjectView), typeof (string), typeof (string), typeof (object)}, 
                    (ObjectView) e.View, e.ItemType, e.ItemName, e.Item);
            foreach (var result in appearanceRules.OfType<AppearanceRule>()) {
                var conditionalAppearanceItems = result.Validate(e.ContextObjects, e.EvaluatorContextDescriptor);
                e.AppearanceObject.Items.AddRange(conditionalAppearanceItems);
            }
        }

        private void AppearanceControllerOnCustomGetIsRulePropertiesEmpty(object sender, CustomGetIsRulePropertiesEmptyEventArgs e){
            var appearanceRuleProperties = e.RuleProperties as CachedAppearanceRuleProperties;
            if (appearanceRuleProperties != null)
                e.IsEmpty = !appearanceRuleProperties.Properties.HasValue(new[] { typeof(IModelAppearanceFont) });
        }

        private void AppearanceControllerOnCustomCreateAppearanceRule(object sender, CustomCreateAppearanceRuleEventArgs e){
            var propertyInfo = e.RuleProperties.GetType().PropertyX("Attribute");
            if (propertyInfo!=null&& e.RuleProperties.GetPropertyValue("Attribute") == null) {
                var xpandAppearanceRuleProperties = e.RuleProperties as IXpandAppearanceRuleProperties;
                if (xpandAppearanceRuleProperties != null)
                    e.Rule = new AppearanceRule(new CachedAppearanceRuleProperties(xpandAppearanceRuleProperties), View.ObjectSpace);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IAppearanceRuleProperties, IXpandAppearanceRuleProperties>();
        }
    }

    public class AppearanceRule : DevExpress.ExpressApp.ConditionalAppearance.AppearanceRule {
        private readonly CachedAppearanceRuleProperties _properties;

        public AppearanceRule(CachedAppearanceRuleProperties properties, IObjectSpace objectSpace)
            : base(properties, objectSpace) {
            _properties = properties;
        }

        public CachedAppearanceRuleProperties Properties {
            get { return _properties; }
        }

        internal IList<IConditionalAppearanceItem> Validate(object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
            var ruleValid = (bool)this.CallMethod("GetRuleValid", new[] { typeof(object[]), typeof(EvaluatorContextDescriptor) }, new object[] { contextObjects, evaluatorContextDescriptor });
            var result = new List<IConditionalAppearanceItem>();
            AppearanceState state = ruleValid ? AppearanceState.CustomValue : AppearanceState.ResetValue;
            if (Properties.Properties.HasValue(typeof(IModelAppearanceFont))) {
                result.Add(new AppearanceItemFont(state, Properties.Priority, Properties.Properties));
            }
            return result;
        }

    }

    public class AppearanceItemFont : AppearanceItemBase {
        private IModelAppearanceFont _modelAppearanceFont;
        public AppearanceItemFont(AppearanceState state, int priority, IModelAppearanceFont modelAppearanceFont)
            : base(state, priority) {
            if (state == AppearanceState.CustomValue) {
                _modelAppearanceFont = modelAppearanceFont;
            }
        }
        public IModelAppearanceFont ModelAppearanceFont {
            get { return _modelAppearanceFont; }
            set {
                State = value != null ? AppearanceState.CustomValue : AppearanceState.ResetValue;
                _modelAppearanceFont = value;
            }
        }
        public override bool IsCombineValue {
            get {
                return true;
            }
        }

        protected override void ApplyCore(object targetItem) {
            var appearanceFormat = targetItem as IAppearanceFormat;
            if (appearanceFormat != null && State != AppearanceState.None) {
                if (State == AppearanceState.CustomValue) {
                    var propertyNames = new[] { "Control.Font", "AppearanceObject.Font" };
                    foreach (var propertyName in propertyNames) {
                        var infoAndValue = GetPropertyInfoAndValue(appearanceFormat, propertyName);
                        if (infoAndValue != null && infoAndValue.Item1 != null) {
                            var fontBuilder = new FontBuilder(ModelAppearanceFont, infoAndValue.Item3);
                            infoAndValue.Item1.Set(infoAndValue.Item2, fontBuilder.GetFont());
                            break;
                        }
                    }
                }
                else {
                    appearanceFormat.ResetFontStyle();
                }
            }
        }

        private Tuple<PropertyInfo, object, Font> GetPropertyInfoAndValue(IAppearanceFormat appearanceFormat, string propertyName) {
            var type = appearanceFormat.GetType();
            object obj = appearanceFormat;
            while (propertyName.Contains(".")) {
                var name = propertyName.Substring(0, propertyName.IndexOf(".", StringComparison.Ordinal));
                PropertyInfo propertyInfo;
                try{
                    propertyInfo = type.PropertyX(name, Flags.ExcludeHiddenMembers|Flags.InstancePublic);
                }
                catch (AmbiguousMatchException e){
                    throw new AmbiguousMatchException(type.FullName+", "+propertyName,e);
                }
                if (propertyInfo == null)
                    return null;
                propertyName = propertyName.Substring(propertyName.IndexOf(".", StringComparison.Ordinal) + 1);
                type = propertyInfo.PropertyType;
                obj = propertyInfo.Get(obj);
            }
            var property = type.Property(propertyName);
            return obj != null? (property != null? new Tuple<PropertyInfo, object, Font>(property, obj, (Font) property.Get(obj)): null): null;
        }
    }

    public class CachedAppearanceRuleProperties : DevExpress.ExpressApp.ConditionalAppearance.CachedAppearanceRuleProperties {
        private readonly IXpandAppearanceRuleProperties _properties;

        public CachedAppearanceRuleProperties(IXpandAppearanceRuleProperties properties)
            : base(properties) {
            FontName = properties.FontName;
            Size = Size;
            _properties = properties;
        }

        public IXpandAppearanceRuleProperties Properties {
            get { return _properties; }
        }

        public string FontName { get; set; }

        public float? Size { get; set; }
    }

    public interface IXpandAppearanceRuleProperties : IModelAppearanceFont, IAppearanceRuleProperties {
    }
}
