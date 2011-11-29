using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleManager<TLogicRule> : ILogicRuleManager<TLogicRule> 
    {
        public const BindingFlags MethodRuleBindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic;

        



        static IValueManager<LogicRuleManager<TLogicRule>>
            instanceManager;

        readonly Dictionary<ITypeInfo, List<TLogicRule>> rules;

        protected LogicRuleManager() {
            rules = new Dictionary<ITypeInfo, List<TLogicRule>>();
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
                    List<TLogicRule> result = GetEmptyRules();
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
        #endregion
        IEnumerable<TLogicRule> GetTypeRules(ITypeInfo typeInfo) {
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