using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.RuleModeller
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ModelRuleAttribute : Attribute, IModelRule
    {
        protected ModelRuleAttribute(string id,Nesting targetViewNesting, string normalCriteria, string emptyCriteria, ViewType viewType,string viewId)
        {
            ViewId = viewId;
            ID=id;
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
        public string ID { get; set; }

        /// <summary>
        /// Criteria to apply when show ListView empty
        /// </summary>
        public string EmptyCriteria { get; set; }

        

        public string Description { get; set; }
        ITypeInfo IModelRule.TypeInfo { get; set; }
        public string ViewId { get; set; }
    }
}