using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo
{
    public static class CriteriaOperatorExtensions
    {
        public static CriteriaOperator GetClassTypeFilter(this Type type, Session session, string path)
        {
            path = path.TrimEnd('.');
            XPClassInfo xpClassInfo = session.GetClassInfo(type);
            XPObjectType xpObjectType = session.GetObjectType(xpClassInfo);
            string propertyName = path + "." + XPObject.Fields.ObjectType.PropertyName;
            return
                new GroupOperator(GroupOperatorType.Or, new NullOperator(propertyName),
                                  new BinaryOperator(propertyName,
                                                     xpObjectType));
        }

        public static CriteriaOperator GetClassTypeFilter(this Type type, Session session)
        {
            XPClassInfo xpClassInfo = session.GetClassInfo(type);
            XPObjectType xpObjectType = session.GetObjectType(xpClassInfo);

            return XPObject.Fields.ObjectType.IsNull() |
                   XPObject.Fields.ObjectType == new OperandValue(xpObjectType.Oid);
        }
        public static CriteriaOperator Parse(string propertyPath, CriteriaOperator criteriaOperator)
        {
            while (propertyPath.IndexOf(".")>-1)
            {
                propertyPath = propertyPath.Substring(0, propertyPath.IndexOf(".")) + "[" +
                               propertyPath.Substring(propertyPath.IndexOf(".") + 1) + "]";
            }
//            string replace = criteriaOperator.ToString().Replace("[","").Replace("]","").Replace(" ","");
            for (int i = propertyPath.Length-1; i > -1; i--)
                if (propertyPath[i] != ']')
                {
                    propertyPath = propertyPath.Substring(0, i+1) + "[" + criteriaOperator.ToString() + "]" +
                                   new string(']', propertyPath.Length - i-1);
                    break;
                }
            
            return CriteriaOperator.Parse(propertyPath);
        }
    }
}
