using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleManager<TLogicRule> : ILogicRuleManager<TLogicRule> {
        public const BindingFlags MethodRuleBindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic;

        static IValueManager<LogicRuleManager<TLogicRule>>
            instanceManager;

        readonly Dictionary<Type, List<TLogicRule>> rules;

        protected LogicRuleManager() {
            rules = new Dictionary<Type, List<TLogicRule>>();
        }

        public static LogicRuleManager<TLogicRule> Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.GetValueManager<LogicRuleManager<TLogicRule>>("LogicRuleManager");
                }
                return instanceManager.Value ?? (instanceManager.Value = new LogicRuleManager<TLogicRule>());
            }
        }
        #region ILogicRuleManager<TLogicRule> Members
        public List<TLogicRule> this[ITypeInfo typeInfo] {
            get {
                lock (rules) {
                    return GetLogicRules(typeInfo.Type);
                }
            }
            set {
                lock (rules) {
                    rules[typeInfo.Type] = value;
                }
            }
        }

        List<TLogicRule> GetLogicRules(Type type) {
            List<TLogicRule> result = GetEmptyRules();
            while (type != null && type != typeof(object)) {
                result.AddRange(GetTypeRules(type));
                type = type.BaseType;
            }
            return result;
        }
        #endregion
        IEnumerable<TLogicRule> GetTypeRules(Type typeInfo) {
            List<TLogicRule> result;
            if (!rules.TryGetValue(typeInfo, out result)) {
                result = GetEmptyRules();
                rules.Add(typeInfo, result);
            }
            return result ?? GetEmptyRules();
        }

        List<TLogicRule> GetEmptyRules() {
            return new List<TLogicRule>();
        }

        public static bool HasRules(ITypeInfo typeInfo) {
            foreach (var rule in Instance.rules) {
                if (rule.Key.Name == "IOrder") {
                    Debug.Print("");
                }
            }
            return Instance[typeInfo].Count > 0;
        }

        public static bool HasRules(View view) {
            if (view != null && view.ObjectTypeInfo != null) {
                return HasRules(view.ObjectTypeInfo);
            }
            return false;
        }


        public static bool IsDisplayedMember(IMemberInfo memberInfo) {
            if (memberInfo.Owner.KeyMember == memberInfo) {
                return true;
            }
            return memberInfo.IsPublic && memberInfo.IsVisible && !memberInfo.IsStatic &&
                   (memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
        }


        public static IEnumerable<TLogicRule> FindAttributes(ITypeInfo typeInfo) {
            return typeInfo != null ? GetLogicRuleAttributes(typeInfo) : null;
        }

        static IEnumerable<TLogicRule> GetLogicRuleAttributes(ITypeInfo typeInfo) {
            return typeInfo.FindAttributes<Attribute>(false).OfType<TLogicRule>();
        }


    }
}