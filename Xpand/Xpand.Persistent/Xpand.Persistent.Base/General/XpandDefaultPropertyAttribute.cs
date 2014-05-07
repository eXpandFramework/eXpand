using System;

namespace Xpand.Persistent.Base.General{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XpandDefaultPropertyAttribute : Attribute{
        private readonly string _expression;
        private readonly string _memberName = "DisplayName";
        private bool _inVisibleInAllViews = true;

        public XpandDefaultPropertyAttribute(string expression){
            _expression = expression;
        }

        public XpandDefaultPropertyAttribute(string expression, string memberName){
            _expression = expression;
            _memberName = memberName;
        }

        public bool InVisibleInAllViews{
            get { return _inVisibleInAllViews; }
            set { _inVisibleInAllViews = value; }
        }

        public string MemberName{
            get { return _memberName; }
        }

        public string Expression{
            get { return _expression; }
        }
    }
}