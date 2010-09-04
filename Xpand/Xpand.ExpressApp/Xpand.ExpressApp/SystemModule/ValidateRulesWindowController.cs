using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.Metadata;
using System.Linq;

namespace Xpand.ExpressApp.SystemModule
{
    public class ValidateRulesWindowController : WindowController
    {

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            if (Debugger.IsAttached)
            {
                IEnumerable<XPClassInfo> collection = XafTypesInfo.XpoTypeInfoSource.XPDictionary.Classes.Cast<XPClassInfo>();
                foreach (var typeInfo in collection)
                    if (typeInfo.HasAttribute(typeof(RuleCombinationOfPropertiesIsUniqueAttribute)))
                        if (typesInfo.FindTypeInfo(typeInfo.ClassType).DefaultMember == null)
                            throw new NullReferenceException("DefaultMember of " + typeInfo.FullName + " is null");
            }
        }

    }
}