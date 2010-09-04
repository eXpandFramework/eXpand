using System;
using System.Reflection;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo
{
    public static class XPDictionaryExtensions
    {
        public static XPClassInfo CreateClass(this XPDictionary xpDictionary,Assembly assembly, XPClassInfo baseClassInfo, string className, params Attribute[] attributes)
        {
            return new XpandXPClassInfo(xpDictionary,assembly, baseClassInfo, className, attributes);
        }
    }
}
