using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.Linq;
using eXpand.ExpressApp.Security.Attributes;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.Security.Calculators
{
    public class ActivationCalculator
    {
        public const string TargetPropertiesDelimiters = ",:; ";
        private static readonly object dummyObject = new object();
        private static readonly ActivationCalculator singletonInstance;

        public struct TypeStruct
        {
            public Type ObjectType { get; set; }

            public Type ActivationRuleType { get; set; }
        }
        private readonly Dictionary<TypeStruct, List<ActivationRuleAttributeMethodInfo>> attributesCore =
            new Dictionary<TypeStruct, List<ActivationRuleAttributeMethodInfo>>();

        static ActivationCalculator()
        {
            singletonInstance = new ActivationCalculator();
        }

        private ActivationCalculator()
        {
        }
        

            
        public List<ActivationRuleAttributeMethodInfo> this[Type objectType,Type activationRuleAttributeType]
        {
            get
            {
                lock (new object())
                {
                    var key = new TypeStruct { ActivationRuleType = activationRuleAttributeType, ObjectType = objectType };
                    if (!attributesCore.ContainsKey(key))
                    {
                        object invoke =GetType().GetMethod("FindEditorStateAttributes").MakeGenericMethod(new[]
                                                                                                              {
                                                                                                                  activationRuleAttributeType
                                                                                                              }).Invoke(this, new object[]
                                                                                                                                  {
                                                                                                                                      objectType
                                                                                                                                  }
                            );

                        attributesCore.Add(key, (List<ActivationRuleAttributeMethodInfo>)invoke);
                    }

                    return attributesCore[key];
                }
            }
        }

        public static ActivationCalculator Instance
        {
            get { return singletonInstance; }
        }

        private static bool ComputeControllerActivation(
            ActivationRuleAttributeMethodInfo pair, object targetObject, string criteria)
        {
            Guard.ArgumentNotNull(targetObject, "targetObject");
            Guard.ArgumentNotNull(pair, "pair");

            MethodInfo methodInfo = pair.MethodInfo;

            if (methodInfo != null)
            {
                var parameters = new object[1];
                var result = (bool) methodInfo.Invoke(targetObject, parameters);
                var useMethodResult = (bool) parameters[0];
                if (useMethodResult)
                    return result;
                return true;
            }
            return ComputeControllerActivation(targetObject, criteria);
        }

        public static bool ComputeControllerActivation(object targetObject, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                return true;
            return
                new ExpressionEvaluator(new EvaluatorContextDescriptorDefault(targetObject.GetType()),
                                        CriteriaOperator.Parse(criteria)).Fit(targetObject);
        }

        public static bool IsActive(object targetObject,
                                    ActivationRuleAttributeMethodInfo pair)
        {
            Guard.ArgumentNotNull(targetObject, "targetObject");
            Guard.ArgumentNotNull(pair, "attribute");

            return ComputeControllerActivation(pair, targetObject, pair.ActivationRule.NormalCriteria);
        }

        public static bool IsActive(ActivationRuleAttributeMethodInfo pair)
        {
            Guard.ArgumentNotNull(pair, "attribute");
            return ComputeControllerActivation(pair, dummyObject, pair.ActivationRule.EmptyCriteria);
        }

        public class ActivationRuleAttributeMethodInfo
        {
            

            public ActivationRuleAttributeMethodInfo(ActivationRuleAttribute ruleAttribute, MethodInfo methodInfo)
            {
                ActivationRule = ruleAttribute;
                MethodInfo = methodInfo;
            }

            public ActivationRuleAttribute ActivationRule { get; set; }
            public MethodInfo MethodInfo { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ActivationRuleAttributeMethodInfo> FindEditorStateAttributes<activationRuleAttribute>(Type type) where activationRuleAttribute : ActivationRuleAttribute
        {
            Tracing.Tracer.LogSeparator(null);
            Tracing.Tracer.LogSeparator(string.Format("Begin of collecting attributes from methods of the {0} type.",
                                                      type.FullName));
            var dictionary = new List<ActivationRuleAttributeMethodInfo>();
            foreach (
                MethodInfo methodInfo in
                    type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly))
            {
                if (methodInfo.DeclaringType != type)
                    continue;
                if (methodInfo.ReturnType != typeof (bool))
                {
                    Tracing.Tracer.LogWarning("Method {0} of the {1} type should not be generic.", methodInfo.Name,
                                              type.FullName);
                    continue;
                }
                if (methodInfo.GetParameters().Length != 1)
                {
                    Tracing.Tracer.LogWarning("Method {0} of the {1} type should be parameterless.", methodInfo.Name,
                                              type.FullName);
                    continue;
                }
                if (methodInfo.ContainsGenericParameters)
                {
                    Tracing.Tracer.LogWarning("Method {0} of the {1} type should return a value of the {2} type.",
                                              methodInfo.Name, type.FullName, typeof (bool).FullName);
                    continue;
                }
                if (methodInfo.GetParameters().Length == 1)
                {
                    ParameterInfo pi = methodInfo.GetParameters()[0];
                    if (pi.ParameterType.FullName != typeof (Boolean).FullName + "&")
                    {
                        continue;
                    }
                }
//                var attributeType =controllers? typeof (ControllerActivationRuleAttribute):typeof(ActionActivationRuleAttribute);
                var attributeType = typeof(activationRuleAttribute);
                object[] attributes = methodInfo.GetCustomAttributes(attributeType, true);
                foreach (ActivationRuleAttribute attribute in attributes)
                    if (!IsFromBaseType<activationRuleAttribute>(methodInfo) )
                        dictionary.Add(new ActivationRuleAttributeMethodInfo(attribute,methodInfo));
            }
//            var activationRuleAttributes = controllers
//                                               ? (IEnumerable) XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<ControllerActivationRuleAttribute>()
//                                               : XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<ActionActivationRuleAttribute>();
            var activationRuleAttributes =
                XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<activationRuleAttribute>();
            
            List<activationRuleAttribute> attributesRules = activationRuleAttributes.ToList();
            for (int i = 0; i < attributesRules.Count; i++)
            {
                activationRuleAttribute attribute = attributesRules[i];
                
                dictionary.Add(new ActivationRuleAttributeMethodInfo(attribute, null));
            }
            
            Tracing.Tracer.LogSeparator(null);
            Tracing.Tracer.LogSeparator(
                string.Format(
                    "End of collecting attributes from the methods of the {0} type. Summary: {1} attributes are collected.",
                    type.FullName, dictionary.Count));
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool IsFromBaseType<activationRuleAttribute>(MethodInfo methodInfo) where activationRuleAttribute : ActivationRuleAttribute
        {
            if (methodInfo.DeclaringType.BaseType != null)
            {
                MethodInfo baseMethodInfo = methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name);
                if (baseMethodInfo != null)
                {
//                    var attributeType =controllers? typeof (ControllerActivationRuleAttribute):typeof(ActionActivationRuleAttribute);
                    var attributeType = typeof(activationRuleAttribute);
                    if (baseMethodInfo.GetCustomAttributes(attributeType, true).Length > 0)
                        return true;
                }
            }
            return false;
        }

        public static void ActivationCalculated(Controller controller,  Action<bool, ActivationRuleAttribute> active,Type actiovationRuleAttributeType)
        {
            var view = controller.Frame.View;
            List<ActivationRuleAttributeMethodInfo> instance = Instance[view.ObjectTypeInfo.Type, actiovationRuleAttributeType];
            foreach (var pair in instance)
            {
                ActivationRuleAttribute activationRuleAttribute = pair.ActivationRule;
                if (view is DetailView) detailViewActivationCalculated(view, pair, activationRuleAttribute, active);
                else listViewActivationCalculated(view, pair, activationRuleAttribute, active);
            }
        }

        private static void listViewActivationCalculated(View view, ActivationRuleAttributeMethodInfo pair,
                                                         ActivationRuleAttribute activationRuleAttribute,
                                                         Action<bool, ActivationRuleAttribute> active)
        {
            if (IsListViewTargeted(activationRuleAttribute, view))
            {
                bool isActive = view.CurrentObject == null ? IsActive(pair) : IsActive(view.CurrentObject, pair);
                active(isActive,activationRuleAttribute);
            }
        }

        public static bool IsListViewTargeted(IActivationRule activationRuleAttribute, View view)
        {
            return (activationRuleAttribute.ViewType == ViewType.ListView || activationRuleAttribute.ViewType == ViewType.Any) && view is ListView &&
                   ((activationRuleAttribute.Nesting == Nesting.Root && view.IsRoot) ||
                    (activationRuleAttribute.Nesting == Nesting.Nested && view.IsRoot == false) ||
                    (activationRuleAttribute.Nesting == Nesting.Any));
        }

        private static void detailViewActivationCalculated(View view, ActivationRuleAttributeMethodInfo pair, ActivationRuleAttribute activationRuleAttribute, Action<bool, ActivationRuleAttribute> active)
        {
            if (IsDetailViewTargeted(view, activationRuleAttribute))
            {
                bool isActive = IsActive(view.CurrentObject, pair);
                active(isActive, activationRuleAttribute);
            }
        }

        public static bool IsDetailViewTargeted(View view, IActivationRule activationRule)
        {
            return view.CurrentObject != null &&
                   ((activationRule.ViewType == ViewType.DetailView || activationRule.ViewType == ViewType.Any) && view is DetailView);
        }
    }
}