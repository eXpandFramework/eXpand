using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleManager {
        public const BindingFlags MethodRuleBindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic;

        static IValueManager<LogicRuleManager> _instanceManager;

        readonly Dictionary<ITypeInfo, List<LogicRule>> rules;

        LogicRuleManager() {
            rules = new Dictionary<ITypeInfo, List<LogicRule>>();
        }

        internal Dictionary<ITypeInfo, List<LogicRule>> Rules {
            get { return rules; }
        }

        public static LogicRuleManager Instance {
            get {
                if (_instanceManager == null) {
                    _instanceManager = ValueManager.GetValueManager<LogicRuleManager>("LogicRuleManager");
                }
                return _instanceManager.Value ?? (_instanceManager.Value = new LogicRuleManager());
            }
        }

        public List<LogicRule> this[ITypeInfo typeInfo] {
            get {
                lock (rules) {
                    if (typeInfo!=null) {
                        if (!rules.ContainsKey(typeInfo))
                            rules.Add(typeInfo, new List<LogicRule>());
                        return rules[typeInfo];
                    }
                    return new List<LogicRule>();
                }
            }
        }
        
        public static bool HasRules(ITypeInfo typeInfo) {
            return Instance[typeInfo].Any() ;
        }
        
        public static bool HasRules(View view) {
            return view != null && view.ObjectTypeInfo != null && HasRules(view.ObjectTypeInfo);
        }

        public static bool IsDisplayedMember(IMemberInfo memberInfo) {
            if (memberInfo.Owner.KeyMember == memberInfo) {
                return true;
            }
            return memberInfo.IsPublic && memberInfo.IsVisible &&
                   (memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
        }


        public static IEnumerable<ILogicRule> FindAttributes(ITypeInfo typeInfo) {
            return typeInfo != null ? GetLogicRuleAttributes(typeInfo) : null;
        }

        static IEnumerable<ILogicRule> GetLogicRuleAttributes(ITypeInfo typeInfo) {
            return typeInfo.FindAttributes<Attribute>(false).OfType<ILogicRule>();
        }
    }
}