using System;

namespace eXpand.ExpressApp.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterRecordAttribute : Attribute
    {
        public FilterRecordAttribute(string targetObjectCriteria)
        {
            this.targetObjectCriteria = targetObjectCriteria;
        }

        public FilterRecordAttribute(bool excludeAdmin, string targetObjectCriteria)
        {
            this.excludeAdmin = excludeAdmin;
            this.targetObjectCriteria = targetObjectCriteria;
        }

        private readonly bool excludeAdmin;
        public bool ExcludeAdmin
        {
            get { return excludeAdmin; }
        }
        
        private readonly string targetObjectCriteria;
        public string TargetObjectCriteria
        {
            get { return targetObjectCriteria; }
        }
    }
}