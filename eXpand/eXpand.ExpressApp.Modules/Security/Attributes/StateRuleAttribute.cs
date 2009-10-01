using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Security.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class StateRuleAttribute : Attribute, IStateRule
    {
        protected StateRuleAttribute(string id,Nesting targetViewNesting, string normalCriteria, string emptyCriteria, ViewType viewType,State state,string viewId)
        {
            this.state=state;
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

        private State state;
        public virtual State State{
            get { return state; }
            set { state = value; }
        }

        public string Description { get; set; }
        ITypeInfo IStateRule.TypeInfo { get; set; }
        public string ViewId { get; set; }
    }
}