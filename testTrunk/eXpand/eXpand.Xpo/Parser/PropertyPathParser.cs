using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Parser
{
    public class PropertyPathParser
    {
        private readonly XPClassInfo _xpClassInfo;

        public PropertyPathParser(XPClassInfo type)
        {
            _xpClassInfo = type;
        }

        public CriteriaOperator Parse(string propertyPath, string parameters)
        {
            
            XPMemberInfo memberInfo = ReflectorHelper.GetXpMemberInfo(_xpClassInfo, propertyPath);
            string provider = "{0}=?";
            if (memberInfo.IsCollection)
            {
                propertyPath="["+propertyPath+"]["+parameters+"]";
                provider = propertyPath;
            }
            
            return CriteriaOperator.Parse(string.Format(provider, propertyPath), parameters);
        }
    }
}