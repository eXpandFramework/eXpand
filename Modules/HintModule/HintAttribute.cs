using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.HintModule
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HintAttribute : Attribute
    {
        private readonly string hint;
        private readonly string hintPropertyName;
        private readonly ViewType targetViewType;

        public HintAttribute(string hint, ViewType targetViewType)
            : this(hint, targetViewType, "")
        {
        }

        public HintAttribute(string hint, ViewType targetViewType, string hintPropertyName)
        {
            this.hint = hint;
            this.targetViewType = targetViewType;
            this.hintPropertyName = hintPropertyName;
        }

        public HintAttribute(string hint)
            : this(hint, ViewType.ListView)
        {
        }

        public string Hint
        {
            get { return hint; }
        }

        public ViewType TargetViewType
        {
            get { return targetViewType; }
        }

        public string HintPropertyName
        {
            get { return hintPropertyName; }
        }

        public bool IsTargetView(View view)
        {
            return
                TargetViewType == ViewType.Any
                ||
                ((view is DetailView) && (TargetViewType == ViewType.DetailView))
                ||
                ((view is ListView) && (TargetViewType == ViewType.ListView));
        }
    }
}