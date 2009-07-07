using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ActivationRuleAttribute : Attribute, IActivationRule
    {
        protected ActivationRuleAttribute(Nesting targetViewNesting, string normalCriteria, string emptyCriteria, ViewType viewType,string module)
        {
            ViewType = viewType;
            Nesting = targetViewNesting;
            NormalCriteria = normalCriteria;
            EmptyCriteria = emptyCriteria;
        }

        public ViewType ViewType { get; set; }

        /// <summary>
        /// Nesting of ListView, but not works in MainWindow...(Root)...
        /// </summary>
        public Nesting Nesting { get; set; }

        /// <summary>
        /// Criteria to apply when show DetailView or filled ListView 
        /// </summary>
        public string NormalCriteria { get; set; }

        /// <summary>
        /// Criteria to apply when show ListView empty
        /// </summary>
        public string EmptyCriteria { get; set; }

        
    }
}