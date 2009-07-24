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
    public class StateCalculator
    {
        public const string TargetPropertiesDelimiters = ",:; ";
        private static readonly object dummyObject = new object();
        private static readonly StateCalculator singletonInstance;

        public struct TypeStruct
        {
            public Type ObjectType { get; set; }

            public Type StateRuleType { get; set; }
        }
        private readonly Dictionary<TypeStruct, List<StateRuleAttributeMethodInfo>> attributesCore =
            new Dictionary<TypeStruct, List<StateRuleAttributeMethodInfo>>();

        static StateCalculator()
        {
            singletonInstance = new StateCalculator();
        }

        private StateCalculator()
        {
        }
        

            
        public List<StateRuleAttributeMethodInfo> this[Type objectType,Type stateRuleAttributeType]
        {
            get
            {
                lock (new object())
                {
                    var key = new TypeStruct { StateRuleType = stateRuleAttributeType, ObjectType = objectType };
                    if (!attributesCore.ContainsKey(key))
                    {
                        object invoke =GetType().GetMethod("FindEditorStateAttributes").MakeGenericMethod(new[]
                                                                                                              {
                                                                                                                  stateRuleAttributeType
                                                                                                              }).Invoke(this, new object[]
                                                                                                                                  {
                                                                                                                                      objectType
                                                                                                                                  }
                            );

                        attributesCore.Add(key, (List<StateRuleAttributeMethodInfo>)invoke);
                    }

                    return attributesCore[key];
                }
            }
        }

        public static StateCalculator Instance
        {
            get { return singletonInstance; }
        }

        private static bool ComputeControllerState(
            StateRuleAttributeMethodInfo pair, object targetObject, string criteria)
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
            return ComputeControllerState(targetObject, criteria);
        }

        public static bool ComputeControllerState(object targetObject, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                return true;
            return
                new ExpressionEvaluator(new EvaluatorContextDescriptorDefault(targetObject.GetType()),
                                        CriteriaOperator.Parse(criteria)).Fit(targetObject);
        }

        public static bool IsActive(object targetObject,
                                    StateRuleAttributeMethodInfo pair)
        {
            Guard.ArgumentNotNull(targetObject, "targetObject");
            Guard.ArgumentNotNull(pair, "attribute");

            return ComputeControllerState(pair, targetObject, pair.StateRule.NormalCriteria);
        }

        public static bool IsActive(StateRuleAttributeMethodInfo pair)
        {
            Guard.ArgumentNotNull(pair, "attribute");
            return ComputeControllerState(pair, dummyObject, pair.StateRule.EmptyCriteria);
        }

        public class StateRuleAttributeMethodInfo
        {
            

            public StateRuleAttributeMethodInfo(StateRuleAttribute ruleAttribute, MethodInfo methodInfo)
            {
                StateRule = ruleAttribute;
                MethodInfo = methodInfo;
            }

            public StateRuleAttribute StateRule { get; set; }
            public MethodInfo MethodInfo { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<StateRuleAttributeMethodInfo> FindEditorStateAttributes<stateRuleAttribute>(Type type) where stateRuleAttribute : StateRuleAttribute
        {
            Tracing.Tracer.LogSeparator(null);
            Tracing.Tracer.LogSeparator(string.Format("Begin of collecting attributes from methods of the {0} type.",
                                                      type.FullName));
            var dictionary = new List<StateRuleAttributeMethodInfo>();
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
                var attributeType = typeof(stateRuleAttribute);
                object[] attributes = methodInfo.GetCustomAttributes(attributeType, true);
                foreach (StateRuleAttribute attribute in attributes)
                    if (!IsFromBaseType<stateRuleAttribute>(methodInfo) )
                        dictionary.Add(new StateRuleAttributeMethodInfo(attribute,methodInfo));
            }
//            var activationRuleAttributes = controllers
//                                               ? (IEnumerable) XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<ControllerActivationRuleAttribute>()
//                                               : XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<ActionActivationRuleAttribute>();
            var stateRuleAttributes =
                XafTypesInfo.Instance.FindTypeInfo(type).FindAttributes<stateRuleAttribute>();
            
            List<stateRuleAttribute> attributesRules = stateRuleAttributes.ToList();
            for (int i = 0; i < attributesRules.Count; i++)
            {
                stateRuleAttribute attribute = attributesRules[i];
                
                dictionary.Add(new StateRuleAttributeMethodInfo(attribute, null));
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
        private static bool IsFromBaseType<stateRuleAttribute>(MethodInfo methodInfo) where stateRuleAttribute : StateRuleAttribute
        {
            if (methodInfo.DeclaringType.BaseType != null)
            {
                MethodInfo baseMethodInfo = methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name);
                if (baseMethodInfo != null)
                {
//                    var attributeType =controllers? typeof (ControllerActivationRuleAttribute):typeof(ActionActivationRuleAttribute);
                    var attributeType = typeof(stateRuleAttribute);
                    if (baseMethodInfo.GetCustomAttributes(attributeType, true).Length > 0)
                        return true;
                }
            }
            return false;
        }

        public static void StateCalculated(Controller controller,  Action<bool, StateRuleAttribute> active,Type actiovationRuleAttributeType)
        {
            if (controller.Frame != null && controller.Frame.View != null)
            {
                var view = controller.Frame.View;
                List<StateRuleAttributeMethodInfo> instance = Instance[view.ObjectTypeInfo.Type, actiovationRuleAttributeType];
                foreach (var pair in instance)
                {
                    StateRuleAttribute stateRuleAttribute = pair.StateRule;
                    if (view is DetailView) detailViewStateCalculated(view, pair, stateRuleAttribute, active);
                    else listViewStateCalculated(view, pair, stateRuleAttribute, active);
                }
            }
        }

        private static void listViewStateCalculated(View view, StateRuleAttributeMethodInfo pair,
                                                         StateRuleAttribute stateRuleAttribute,
                                                         Action<bool, StateRuleAttribute> active)
        {
            if (IsListViewTargeted(stateRuleAttribute, view))
            {
                bool isActive = view.CurrentObject == null ? IsActive(pair) : IsActive(view.CurrentObject, pair);
                active(isActive,stateRuleAttribute);
            }
        }

        public static bool IsListViewTargeted(IStateRule stateRuleAttribute, View view)
        {
            return (stateRuleAttribute.ViewType == ViewType.ListView || stateRuleAttribute.ViewType == ViewType.Any) && view is ListView &&
                   ((stateRuleAttribute.Nesting == Nesting.Root && view.IsRoot) ||
                    (stateRuleAttribute.Nesting == Nesting.Nested && view.IsRoot == false) ||
                    (stateRuleAttribute.Nesting == Nesting.Any));
        }

        private static void detailViewStateCalculated(View view, StateRuleAttributeMethodInfo pair, StateRuleAttribute stateRuleAttribute, Action<bool, StateRuleAttribute> active)
        {
            if (IsDetailViewTargeted(view, stateRuleAttribute))
            {
                bool isActive = IsActive(view.CurrentObject, pair);
                active(isActive, stateRuleAttribute);
            }
        }

        public static bool IsDetailViewTargeted(View view, IStateRule stateRule)
        {
            return view.CurrentObject != null &&
                   ((stateRule.ViewType == ViewType.DetailView || stateRule.ViewType == ViewType.Any) && view is DetailView);
        }
    }
}