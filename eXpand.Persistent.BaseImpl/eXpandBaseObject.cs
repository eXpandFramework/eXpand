using System;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Utils;

namespace eXpand.Persistent.BaseImpl
{
    [NonPersistent]
    public abstract class eXpandBaseObject : DevExpress.Persistent.BaseImpl.BaseObject,IHideObjectMembers
    {
        


        protected eXpandBaseObject(Session session)
            : base(session)
        {
        }

        

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            ConstractExpandObjectMembers();
        }

        protected virtual void ConstractExpandObjectMembers()
        {
            ConstractExpandObjectMembers(Session, this);
        }

        public static void ConstractExpandObjectMembers(Session session, XPBaseObject xpBaseObject)
        {
            if (session.IsNewObject(xpBaseObject)){
                foreach (XPMemberInfo memberInfo in xpBaseObject.ClassInfo.ObjectProperties){
                    if (memberInfo.HasAttribute(typeof (ExpandObjectMembersAttribute))){
                        if (((ExpandObjectMembersAttribute)
                             memberInfo.GetAttributeInfo(typeof (ExpandObjectMembersAttribute))).ExpandingMode !=ExpandObjectMembers.Never)
                            memberInfo.SetValue(xpBaseObject,Activator.CreateInstance(memberInfo.MemberType,new object[]{xpBaseObject.Session}));
                    }
                }
            }
        }

        
    }
}

