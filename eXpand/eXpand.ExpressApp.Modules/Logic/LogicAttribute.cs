using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class LogicAttribute : Attribute, ILogicRule
    {
        protected LogicAttribute(string id,Nesting targetViewNesting, ViewType viewType,string viewId)
        {
            ViewId = viewId;
            ID=id;
            ViewType = viewType;
            Nesting = targetViewNesting;
            
            
        }


        public ViewType ViewType { get; set; }

        /// <summary>
        /// Nesting of ListView, but not works in MainWindow...(Root)...
        /// </summary>
        public Nesting Nesting { get; set; }

        
        public string ID { get; set; }

        
        

        public string Description { get; set; }
        ITypeInfo ILogicRule.TypeInfo { get; set; }
        public string ViewId { get; set; }
    }
}