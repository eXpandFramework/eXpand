using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class NHPropertyCollectionSource : PropertyCollectionSource
    {
        public NHPropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, object masterObject, IMemberInfo memberInfo, CollectionSourceMode mode)
            : base(objectSpace, masterObjectType, masterObject, memberInfo, false, mode)
        {

        }

        public override void Add(object obj)
        {
            base.Add(obj);
            if (MemberInfo.AssociatedMemberInfo != null)
            {
                var referenceToOwner = MemberInfo.AssociatedMemberInfo;
                if (referenceToOwner != null)
                    referenceToOwner.SetValue(obj, MasterObject);
            }
        }
    }
}
