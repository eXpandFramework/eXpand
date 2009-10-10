using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using System.Linq;

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
            string path = null;
            string criteria = "";
            foreach (string split in propertyPath.Split('.')) {
                path += split;
                criteria += split;
                XPMemberInfo memberInfo = ReflectorHelper.GetXpMemberInfo(_xpClassInfo, path);
                if (memberInfo.IsCollection) {
                    criteria = criteria.TrimEnd('.');
                    criteria += "[";
                }
                if (!(criteria.LastIndexOf('[') == criteria.Length - 1))
                    criteria += ".";
                path += ".";
            }
            criteria += parameters;
            var count = criteria.Where(c => c == '[').Count();
            for (int i = 0; i < count; i++) {
                criteria = criteria + "]";
            }
            return CriteriaOperator.Parse(criteria);
        }

    }
}