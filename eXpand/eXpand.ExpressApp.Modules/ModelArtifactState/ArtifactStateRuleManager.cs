using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.Exceptions;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using eXpand.ExpressApp.ModelArtifactState.StateInfos;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Security.Permissions;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState{
    /// <summary>
    /// A helper class that allows to manage the process of customizing Model Artifacts.
    /// </summary>
    /// 
    public sealed class ArtifactStateRuleManager {
        public const BindingFlags MethodRuleBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public const int MethodRuleOutParametersCount = 1;
        public static readonly Type MethodRuleOutParameterType = typeof(Boolean);
        public static readonly Type MethodRuleReturnType = typeof(State);

        private readonly Dictionary<ITypeInfo, List<ArtifactStateRule>> rules;
        private static IValueManager<ArtifactStateRuleManager> instanceManager;

        public static ArtifactStateRuleManager Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<ArtifactStateRuleManager>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new ArtifactStateRuleManager());
            }
        }
        private ArtifactStateRuleManager() {
            rules = new Dictionary<ITypeInfo, List<ArtifactStateRule>>();
        }
        public List<ArtifactStateRule> this[ITypeInfo typeInfo]
        {
            get {
                lock (rules) {
                    List<ArtifactStateRule> result = GetEmptyRules();
                    while (typeInfo != null && typeInfo.Type != typeof(object)) {
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
        private IEnumerable<ArtifactStateRule> GetTypeRules(ITypeInfo typeInfo)
        {
            List<ArtifactStateRule> result;
            if (!rules.TryGetValue(typeInfo, out result)) {
                result = GetEmptyRules();
                rules.Add(typeInfo, result);
            }
            return result ?? GetEmptyRules();
        }
        private List<ArtifactStateRule> GetEmptyRules()
        {
            return new List<ArtifactStateRule>();
        }
        public static bool NeedsCustomization(ITypeInfo typeInfo) {
            return Instance[typeInfo].Count > 0;
        }
        public static bool NeedsCustomization(View view) {
            if (view != null && view.ObjectTypeInfo != null) {
                return NeedsCustomization(view.ObjectTypeInfo);
            }
            return false;
        }
        /// <summary>
        /// Determines whether a passed object satisfies to the target criteria and the editor's customization according to a given business criteria should be performed.
        /// </summary>
        public static bool Fit(object targetObject, ArtifactStateRule artifactStateRule) {
            string criteria = artifactStateRule.NormalCriteria;
            if (targetObject == null){
                if (string.IsNullOrEmpty(artifactStateRule.EmptyCriteria)){
                    return true;
                }
                return !fit(new object(), artifactStateRule.EmptyCriteria);
            }
            return fit(targetObject, criteria);
        }

        private static bool fit(object targetObject, string criteria){
            Type objectType = targetObject.GetType();
            var wrapper = new LocalizedCriteriaWrapper(objectType, criteria);
            wrapper.UpdateParametersValues();
            var descriptor = new EvaluatorContextDescriptorDefault(objectType);
            var evaluator = new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator);
            return evaluator.Fit(targetObject);
        }

        public static IEnumerable<ArtifactStateRule> FillRulesFromPermissions(XafApplication application,ITypeInfo info)
        {
            if (SecuritySystem.Instance is ISecurityComplex)
                if (SecuritySystem.CurrentUser != null)
                {
                    var permissions = ((IUser)SecuritySystem.CurrentUser).Permissions;
                    foreach (ArtifactStateRulePermission permission in permissions.OfType<ArtifactStateRulePermission>().Where(permission => permission.ObjectType==info.Type))
                    {
                        var rule = (ArtifactStateRule)permission;
                        if (permission is ControllerStateRulePermission){
                            var rulePermission = (ControllerStateRulePermission)permission;
                            if (!(string.IsNullOrEmpty(rulePermission.ControllerType)))
                                ((ControllerStateRule)rule).ControllerType =
                                    application.Modules[0].ModuleManager.ControllersManager.CollectControllers(
                                        typeInfo =>
                                        typeInfo.FullName == rulePermission.ControllerType).
                                        Single().GetType();
                        }
                        else
                        {
                            var rulePermission = (ActionStateRulePermission)permission;
                            ((ActionStateRule)rule).ActionId = rulePermission.ActionId;
                        }
                        yield return rule;
                    }
                }
        }
        public static ArtifactStateInfo CalculateArtifactStateInfo(object targetObject,  ArtifactStateRule rule)
        {
            Guard.ArgumentNotNull(rule, "rule");

            MethodInfo methodInfo = rule.MethodInfo;
            State editorState;
            bool active;

            if (methodInfo != null) {
                editorState = GetEditorStateFromMethod(methodInfo, targetObject, out active);
                if (!string.IsNullOrEmpty(rule.NormalCriteria)) {
                    Tracing.Tracer.LogWarning(
                        ExceptionLocalizerTemplate
                            <ConditionalArtifactStateExceptionResourceLocalizer, ConditionalArtifactStateExceptionId>.
                            GetExceptionMessage(
                            ConditionalArtifactStateExceptionId.BrokenRuleContainsBothCriteriaAndMethodInfoParameters),
                        typeof (ArtifactStateRuleAttribute).Name, rule.TypeInfo.FullName, rule.NormalCriteria,
                        rule.MethodInfo.Name);
                }
            }
            else {
                editorState = rule.State;
                active = Fit(targetObject, rule);
            }
            return new ArtifactStateInfo(active, targetObject,  editorState, rule);
        }
        
        private static State GetEditorStateFromMethod(MethodInfo methodInfo, object targetObject, out bool active) {
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            State editorState;
            if (!methodInfo.IsStatic && targetObject == null) {
                editorState = State.Default;
                active = false;
            }
            else {
                var parameters = new object[1];
                editorState = (State)methodInfo.Invoke(targetObject, parameters);
                active = (bool)parameters[0];
            }
            return editorState;
        }
        public static bool IsDisplayedMember(IMemberInfo memberInfo) {
            if (memberInfo.Owner.KeyMember == memberInfo) {
                return true;
            }
            return memberInfo.IsPublic && memberInfo.IsVisible && !memberInfo.IsStatic &&
                   (memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
        }
        public static IEnumerable<ArtifactStateRuleAttribute> FindAttributes(MethodInfo methodInfo)
        {
            if (methodInfo != null) {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (methodInfo.ReturnType == MethodRuleReturnType
                    && !methodInfo.ContainsGenericParameters
                    && parameters.Length == MethodRuleOutParametersCount
                    ) {
                    ParameterInfo parameter = parameters[0];
                    if (parameter.ParameterType == MethodRuleOutParameterType.MakeByRefType() && parameter.IsOut) {
                        foreach (ArtifactStateRuleAttribute attribute in methodInfo.GetCustomAttributes(typeof(ArtifactStateRuleAttribute), true)){
                            yield return attribute;
                        }
                    }
                }
            }
        }
        public static IEnumerable<ArtifactStateRuleAttribute> FindAttributes(ITypeInfo typeInfo) {
            return typeInfo != null ? typeInfo.FindAttributes<ArtifactStateRuleAttribute>(false) : null;
        }

        public static MethodInfo FindMethodRule(Type type, string methodName) {
            if (type != null && !string.IsNullOrEmpty(methodName)) {
                var parameterModifier = new ParameterModifier(MethodRuleOutParametersCount);
                parameterModifier[0] = true;
                MethodInfo methodInfo = type.GetMethod(methodName, MethodRuleBindingFlags, null, new[] { MethodRuleOutParameterType.MakeByRefType() }, new[] { parameterModifier });
                return methodInfo;
            }
            return null;
        }
    }
}