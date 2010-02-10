using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.RuleModeller {
    public class ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule> : IModelRuleManager<TModelRule> where TModelRuleAttribute : ModelRuleAttribute
        where TModelRuleNodeWrapper : ModelRuleNodeWrapper
        where TModelRuleInfo : ModelRuleInfo<TModelRule>, new()
        where TModelRule : ModelRule {
        public const BindingFlags MethodRuleBindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic;

        public const int MethodRuleOutParametersCount = 1;
        public static readonly Type MethodRuleOutParameterType = typeof (Boolean);
        public static readonly Type MethodRuleReturnType = typeof (State);

        static IValueManager<ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule>>
            instanceManager;

        readonly Dictionary<ITypeInfo, List<TModelRule>> rules;

        ModelRuleManager() {
            rules = new Dictionary<ITypeInfo, List<TModelRule>>();
        }

        public static ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule> Instance {
            get {
                if (instanceManager == null) {
                    instanceManager =
                        ValueManager.CreateValueManager
                            <ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule>>();
                }
                return instanceManager.Value ??
                       (instanceManager.Value =
                        new ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule>());
            }
        }

        public List<TModelRule> this[ITypeInfo typeInfo] {
            get {
                lock (rules) {
                    List<TModelRule> result = GetEmptyRules();
                    while (typeInfo != null && typeInfo.Type != typeof (object)) {
                        result.AddRange(GetTypeRules(typeInfo));
                        typeInfo = typeInfo.Base;
                    }
                    return result;
                }
            }
            set {
                lock (rules) {
                    rules[typeInfo] = value;
                }
            }
        }

        IEnumerable<TModelRule> GetTypeRules(ITypeInfo typeInfo) {
            List<TModelRule> result;
            if (!rules.TryGetValue(typeInfo, out result)) {
                result = GetEmptyRules();
                rules.Add(typeInfo, result);
            }
            return result ?? GetEmptyRules();
        }

        List<TModelRule> GetEmptyRules() {
            return new List<TModelRule>();
        }

        public static bool HasRules(ITypeInfo typeInfo) {
            return Instance[typeInfo].Count > 0;
        }

        public static bool HasRules(View view) {
            if (view != null && view.ObjectTypeInfo != null) {
                return HasRules(view.ObjectTypeInfo);
            }
            return false;
        }

        /// <summary>
        /// Determines whether a passed object satisfies to the target criteria and the editor's customization according to a given business criteria should be performed.
        /// </summary>
        public static bool Fit(object targetObject, TModelRule modelRule) {
            string criteria = modelRule.NormalCriteria;
            return targetObject == null
                       ? string.IsNullOrEmpty(modelRule.EmptyCriteria) || fit(new object(), modelRule.EmptyCriteria)
                       : fit(targetObject, criteria);
        }

        static bool fit(object targetObject, string criteria) {
            Type objectType = targetObject.GetType();
            var wrapper = new LocalizedCriteriaWrapper(objectType, criteria);
            wrapper.UpdateParametersValues();
            var descriptor = new EvaluatorContextDescriptorDefault(objectType);
            var evaluator = new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator);
            return evaluator.Fit(targetObject);
        }

        public static IEnumerable<TModelRule> FillRulesFromPermissions(XafApplication application, ITypeInfo info) {
            return new List<TModelRule>();
//            if (SecuritySystem.Instance is ISecurityComplex)
//                if (SecuritySystem.CurrentUser != null)
//                {
//                    var permissions = ((IUser)SecuritySystem.CurrentUser).Permissions;
//                    foreach (ArtifactStateRulePermission permission in permissions.OfType<ArtifactStateRulePermission>().Where(permission => permission.ObjectType==info.Type))
//                    {
//                        var rule = (ArtifactStateRule)permission;
//                        if (permission is ControllerStateRulePermission){
//                            var rulePermission = (ControllerStateRulePermission)permission;
//                            if (!(string.IsNullOrEmpty(rulePermission.ControllerType)))
//                                ((ControllerStateRule)rule).ControllerType =
//                                    application.Modules[0].ModuleManager.ControllersManager.CollectControllers(
//                                        typeInfo =>
//                                        typeInfo.FullName == rulePermission.ControllerType).
//                                        Single().GetType();
//                        }
//                        else
//                        {
//                            var rulePermission = (ActionStateRulePermission)permission;
//                            ((ActionStateRule)rule).ActionId = rulePermission.ActionId;
//                        }
//                        yield return rule;
//                    }
//                }
        }

        public static TModelRuleInfo CalculateModelRuleInfo(object targetObject, TModelRule modelRule) {
            Guard.ArgumentNotNull(modelRule, "rule");
            bool active = Fit(targetObject, modelRule);
            var modelRuleInfo = new TModelRuleInfo {Active = active, Object = targetObject, Rule = modelRule};
            return modelRuleInfo;
        }

        public static bool IsDisplayedMember(IMemberInfo memberInfo) {
            if (memberInfo.Owner.KeyMember == memberInfo) {
                return true;
            }
            return memberInfo.IsPublic && memberInfo.IsVisible && !memberInfo.IsStatic &&
                   (memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
        }

        public static IEnumerable<TModelRuleAttribute> FindAttributes(MethodInfo methodInfo) {
            if (methodInfo != null) {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (methodInfo.ReturnType == MethodRuleReturnType
                    && !methodInfo.ContainsGenericParameters
                    && parameters.Length == MethodRuleOutParametersCount) {
                    ParameterInfo parameter = parameters[0];
                    if (parameter.ParameterType == MethodRuleOutParameterType.MakeByRefType() && parameter.IsOut) {
                        foreach (
                            TModelRuleAttribute attribute in
                                methodInfo.GetCustomAttributes(typeof (TModelRuleAttribute), true)) {
                            yield return attribute;
                        }
                    }
                }
            }
        }

        public static IEnumerable<TModelRuleAttribute> FindAttributes(ITypeInfo typeInfo) {
            return typeInfo != null ? typeInfo.FindAttributes<TModelRuleAttribute>(false) : null;
        }

        public static MethodInfo FindMethodRule(Type type, string methodName) {
            if (type != null && !string.IsNullOrEmpty(methodName)) {
                var parameterModifier = new ParameterModifier(MethodRuleOutParametersCount);
                parameterModifier[0] = true;
                MethodInfo methodInfo = type.GetMethod(methodName, MethodRuleBindingFlags, null,
                                                       new[] {MethodRuleOutParameterType.MakeByRefType()},
                                                       new[] {parameterModifier});
                return methodInfo;
            }
            return null;
        }
        }
}