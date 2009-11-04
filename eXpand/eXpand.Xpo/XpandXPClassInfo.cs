using System;
using System.Reflection;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo {
    public class XpandXPClassInfo : XPDataObjectClassInfo
    {
        private readonly Assembly _assembly;

        public XpandXPClassInfo(XPDictionary dictionary,Assembly assembly, XPClassInfo baseClass, string className, params Attribute[] attributes)
            : base(dictionary, baseClass, className, attributes) {
            _assembly = assembly;
        }

        public XpandXPClassInfo(XPDictionary dictionary, string className, params Attribute[] attributes)
            : base(dictionary, className, attributes)
        {
        }

        public XpandXPClassInfo(XPClassInfo baseClass, string className, params Attribute[] attributes)
            : base(baseClass, className, attributes)
        {
        }
        public override string AssemblyName
        {
            get
            {
                return _assembly.FullName;
            }
        }
    }
}