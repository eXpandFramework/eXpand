using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Utils;

namespace DevExpress.ExpressApp.ConditionalEditorState.Core {
    /// <summary>
    /// A helper class that allows to manage the process of customizing editors.
    /// </summary>
    public sealed class EditorStateRuleManager {
        public const string PropertiesDelimiters = ",:; ";
        public const string SelectAllString = "*";
        private static EditorStateRuleManager singletonInstance = null;
        private static Dictionary<Type, List<EditorStateRule>> rulesCore = null;
        static EditorStateRuleManager() {
            singletonInstance = new EditorStateRuleManager();
            rulesCore = new Dictionary<Type, List<EditorStateRule>>();
        }
        private EditorStateRuleManager() { }
        public static EditorStateRuleManager Instance {
            get {
                return singletonInstance;
            }
        }
        public List<EditorStateRule> this[Type objectType] {
            get {
                Guard.ArgumentNotNull(objectType, "objectType");
                if (!rulesCore.ContainsKey(objectType)) {
                    rulesCore.Add(objectType, GetRulesFromCode(objectType));
                }
                return rulesCore[objectType];
            }
        }
        /// <summary>
        /// Reflects rules from the methods of a given type.
        /// </summary>
        private static List<EditorStateRule> GetRulesFromCode(Type objectType) {
            List<EditorStateRule> temp = new List<EditorStateRule>();
            foreach (MethodInfo methodInfo in objectType.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)) {
                if (methodInfo.DeclaringType != objectType && !AllowMethodsInheritance) {
                    continue;
                }
                if (methodInfo.ReturnType != typeof(EditorState)) {
                    continue;
                } else if (methodInfo.GetParameters().Length != 1) {
                    continue;
                } else if (methodInfo.ContainsGenericParameters) {
                    continue;
                } else if (methodInfo.GetParameters().Length == 1) {
                    ParameterInfo pi = methodInfo.GetParameters()[0];
                    if (pi.ParameterType.FullName != typeof(Boolean).FullName + "&") {
                        continue;
                    }
                }
                object[] attributes = methodInfo.GetCustomAttributes(typeof(EditorStateRuleAttribute), true);
                foreach (EditorStateRuleAttribute attribute in attributes) {
                    if (!IsFromBaseType(methodInfo)) {
                        temp.Add(new EditorStateRule(objectType, attribute.Properties, attribute.EditorState, null, attribute.ViewType, methodInfo));
                    }
                }
            }
            return temp;
        }
        /// <summary>
        /// Reflects rules from the class node in the application model.
        /// </summary>
        private static List<EditorStateRule> GetRulesFromModel(ClassInfoNodeWrapper clw) {
            List<EditorStateRule> temp = new List<EditorStateRule>();
            DictionaryNode conditionalEditorStateNode = clw.Node.FindChildNode(ConditionalEditorStateNodeWrapper.NodeName);
            if (conditionalEditorStateNode != null) {
                ConditionalEditorStateNodeWrapper conditionalEditorStateNodeWrapper = new ConditionalEditorStateNodeWrapper(conditionalEditorStateNode);
                foreach (EditorStateRuleNodeWrapper rule in conditionalEditorStateNodeWrapper.Rules) {
                    temp.Add(new EditorStateRule(clw.ClassTypeInfo.Type, rule.Properties, rule.EditorState, rule.Criteria, rule.ViewType, null));
                }
            }
            return temp;
        }
        /// <summary>
        /// Reflects rules from the application model.
        /// </summary>
        public static void FillRulesFromModel(Dictionary model) {
            ApplicationNodeWrapper applicatioNodeWrapper = new ApplicationNodeWrapper(model);
            foreach (ClassInfoNodeWrapper clw in applicatioNodeWrapper.BOModel.Classes) {
                List<EditorStateRule> temp = GetRulesFromModel(clw);
                if (temp.Count > 0) {
                    Instance[clw.ClassTypeInfo.Type].AddRange(temp);
                }
            }
        }
        /// <summary>
        /// Removes rules got from the application model.
        /// </summary>
        public static void ClearRulesFromModel() {
            foreach (KeyValuePair<Type, List<EditorStateRule>> rule in rulesCore) {
                for (int i = rule.Value.Count - 1; i >= 0; i--) {
                    if (rule.Value[i].MethodInfo == null) {
                        rule.Value.Remove(rule.Value[i]);
                    }
                }
            }
        }
        public static bool NeedsCustomization(Type objectType) {
            return Instance[objectType].Count > 0;
        }
        /// <summary>
        /// Determines whether a passed object satisfies to the target criteria and the editor's customization according to a given business criteria should be performed.
        /// </summary>
        public static bool Fit(object targetObject, string criteria) {
            if (targetObject == null || string.IsNullOrEmpty(criteria)) {
                return true;
            } else {
                Type objectType = targetObject.GetType();
                LocalizedCriteriaWrapper wrapper = new LocalizedCriteriaWrapper(objectType, criteria);
                wrapper.UpdateParametersValues();
                EvaluatorContextDescriptorDefault descriptor = new EvaluatorContextDescriptorDefault(objectType);
                ExpressionEvaluator evaluator = new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator);
                return evaluator.Fit(targetObject);
            }
        }
        public static EditorStateInfo CalculateEditorStateInfo(object obj, string property, EditorStateRule rule) {
            Guard.ArgumentNotNull(property, "property");
            Guard.ArgumentNotNull(rule, "rule");

            EditorStateInfo info = null;
            if (rule.MethodInfo != null && rule.MethodInfo.IsStatic) {
                info = CalculateEditorStateInfoStaticCore(property, rule);
            } else {
                info = (obj != null) ? CalculateEditorStateInfoInstanceCore(obj, property, rule) : null;
            }
            return info;
        }
        /// <summary>
        /// Gets the editor's state against the current object.
        /// </summary>
        private static EditorStateInfo CalculateEditorStateInfoInstanceCore(object targetObject, string property, EditorStateRule rule) {
            Guard.ArgumentNotNull(targetObject, "targetObject");
            MethodInfo methodInfo = rule.MethodInfo;
            bool active = false;
            EditorState editorState = EditorState.Default;
            Type objectType = targetObject.GetType();

            if (methodInfo != null) {
                object[] parameters = new object[1];
                editorState = (EditorState)methodInfo.Invoke(targetObject, parameters);
                active = (bool)parameters[0];
            } else {
                editorState = rule.EditorState;
                active = Fit(targetObject, rule.Criteria);
            }
            return new EditorStateInfo(active, targetObject, property, editorState, rule);
        }
        /// <summary>
        /// Gets the editor's state against the current object.
        /// </summary>
        private static EditorStateInfo CalculateEditorStateInfoStaticCore(string property, EditorStateRule rule) {
            MethodInfo methodInfo = rule.MethodInfo;
            bool active = false;
            EditorState editorState = EditorState.Default;
            if (methodInfo != null) {
                object[] parameters = new object[1];
                editorState = (EditorState)methodInfo.Invoke(null, parameters);
                active = (bool)parameters[0];
            }
            return new EditorStateInfo(active, null, property, editorState, rule);
        }
        /// <summary>
        /// Gets a list of properties from the string.
        /// </summary>
        public static ReadOnlyCollection<string> ParseProperties(Type objectType, string properties) {
            if (string.IsNullOrEmpty(properties)) {
                throw new ArgumentException(string.Format(EditorStateLocalizer.Active.GetLocalizedString("BrokenRuleEmptyPropertiesParameter"), typeof(EditorStateRuleAttribute).Name, objectType.FullName, PropertiesDelimiters));
            }
            List<string> propertiesList = new List<string>();
            List<string> allPropertiesList = new List<string>();
            bool invert = false;

            if (!string.IsNullOrEmpty(properties)) {
                List<string> result = new List<string>();
                foreach (string propertyName in properties.Split(PropertiesDelimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
                    string trimmedPropertyName = propertyName.Trim();
                    if (trimmedPropertyName != SelectAllString && FindMember(objectType, trimmedPropertyName) == null) {
                        throw new ArgumentException(string.Format(EditorStateLocalizer.Active.GetLocalizedString("BrokenRuleIncorrectPropertiesParameter"), objectType.FullName, trimmedPropertyName, typeof(EditorStateRuleAttribute).Name, PropertiesDelimiters));
                    }
                    if (!result.Contains(trimmedPropertyName)) {
                        if (trimmedPropertyName == SelectAllString) {
                            invert = true;
                        }
                        result.Add(trimmedPropertyName);
                    }
                }
                result.Remove(SelectAllString);
                propertiesList = new List<string>(result);

                if (invert) {
                    foreach (IMemberInfo memberInfo in XafTypesInfo.Instance.FindTypeInfo(objectType).Members) {
                        if (memberInfo.IsVisible && !memberInfo.IsOverride && !memberInfo.IsStatic && !propertiesList.Contains(memberInfo.Name)) {
                            allPropertiesList.Add(memberInfo.Name);
                        }
                    }
                    propertiesList = allPropertiesList;
                }
            }
            return new ReadOnlyCollection<string>(propertiesList);
        }
        public static IMemberInfo FindMember(Type objectType, string property) {
            return XafTypesInfo.Instance.FindTypeInfo(objectType).FindMember(property);
        }
        /// <summary>
        /// Determines whether a given method marked with the EditorStateRuleAttribute in the base type.
        /// </summary>
        private static bool IsFromBaseType(MethodInfo methodInfo) {
            if (methodInfo.DeclaringType.BaseType != null) {
                MethodInfo baseMethodInfo = methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name);
                if (baseMethodInfo != null) {
                    if (baseMethodInfo.GetCustomAttributes(typeof(EditorStateRuleAttribute), true).Length > 0) {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool allowMethodsInheritanceCore = true;
        /// <summary>
        /// Determines whether methods marked with the EditorStateRuleAttribute and declared in the base class would be inherited by the descendant class. By default it is true.
        /// </summary>
        public static bool AllowMethodsInheritance {
            get { return allowMethodsInheritanceCore; }
            set { allowMethodsInheritanceCore = value; }
        }
        private static bool allowTrackObjectChangesCore = false;
        /// <summary>
        /// Determines whether the changes with external objects are tracked and rules are recalculated. By default it is false.
        /// </summary>
        public static bool AllowTrackObjectChanges {
            get { return allowTrackObjectChangesCore; }
            set { allowTrackObjectChangesCore = value; }
        }
    }
}