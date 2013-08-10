using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.Xpo.MetaData;

namespace Xpand.Persistent.Base.Xpo {
    public static class IXPSimpleObjectExtensions {
        public static XpandCollectionMemberInfo CreateCollection(this XPClassInfo classInfo, string propertyName, Type elementType, string criteria, params Attribute[] attributes) {
            var newMemberInfo = new XpandCollectionMemberInfo(classInfo, propertyName, typeof(XPCollection<>).MakeGenericType(elementType), criteria);
            foreach (Attribute attribute in attributes)
                newMemberInfo.AddAttribute(attribute);
            return newMemberInfo;
        }
    }
}
