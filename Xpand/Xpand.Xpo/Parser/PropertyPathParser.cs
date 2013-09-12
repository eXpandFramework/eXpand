using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Linq;

namespace Xpand.Xpo.Parser {
    public class PropertyPathParser {
        private readonly XPClassInfo _xpClassInfo;
        readonly Session _session;

        public PropertyPathParser(XPClassInfo type, Session session) {
            _xpClassInfo = type;
            _session = session;
        }

        public CriteriaOperator Parse(string propertyPath, string parameters) {
            string path = null;
            string criteria = "";
            foreach (string split in propertyPath.Split('.')) {
                path += split;
                criteria += split;
                XPMemberInfo memberInfo = XpandReflectionHelper.GetXpMemberInfo(_xpClassInfo, path);
                if (memberInfo.IsCollection) {
                    criteria = criteria.TrimEnd('.');
                    criteria += "[";
                }
                if (criteria.LastIndexOf('[') != criteria.Length - 1)
                    criteria += ".";
                path += ".";
            }

            var count = criteria.Count(c => c == '[');
            criteria += parameters;
            for (int i = 0; i < count; i++) {
                criteria = criteria + "]";
            }
            
            return _session.ParseCriteria(criteria);
        }

    }
}