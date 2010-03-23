using System;
using System.Linq;
using eXpand.Persistent.Base.General;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Logic {
    public class TypesInfo {
        Type _logicRuleNodeWrapperType;
        Type _logicRuleType;
        Type _logicRulesNodeWrapperType;


        public Type LogicRuleType {
            get { return _logicRuleType; }
        }

        public Type LogicRulesNodeWrapperType {
            get { return _logicRulesNodeWrapperType; }
        }

        public Type LogicRuleNodeWrapperType {
            get { return _logicRuleNodeWrapperType; }
        }


        public void RegisterTypes<TLogicRule>() where TLogicRule : ILogicRule {
            _logicRuleType =AppDomain.CurrentDomain.GetTypes(typeof (LogicRule)).Where(type =>!(type.IsAbstract) &&typeof (TLogicRule).IsAssignableFrom(type)).Single();
            _logicRulesNodeWrapperType =AppDomain.CurrentDomain.GetTypes(typeof (LogicRulesNodeWrapper<TLogicRule>)).Single();
            _logicRuleNodeWrapperType =AppDomain.CurrentDomain.GetTypes(typeof (LogicRuleNodeWrapper)).Where(type=>typeof(TLogicRule).IsAssignableFrom(type)).Single();
        }
    }
}