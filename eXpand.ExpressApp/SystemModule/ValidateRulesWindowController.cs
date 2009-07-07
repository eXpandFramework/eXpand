using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.BaseImpl.Validation.CombinationOfPropertiesIsUnique;
using System.Linq;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class ValidateRulesWindowController : WindowController
    {
        public ValidateRulesWindowController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            if (Debugger.IsAttached)
            {
                IEnumerable<XPClassInfo> collection = XafTypesInfo.XpoTypeInfoSource.XPDictionary.Classes.Cast<XPClassInfo>();
                foreach (var typeInfo in collection)
                    if (typeInfo.HasAttribute(typeof (RuleCombinationOfPropertiesIsUniqueAttribute)))
                        if (typesInfo.FindTypeInfo(typeInfo.ClassType).DefaultMember == null)
                            throw new NullReferenceException("DefaultMember of " + typeInfo.FullName + " is null");
            }
        }

    }
}