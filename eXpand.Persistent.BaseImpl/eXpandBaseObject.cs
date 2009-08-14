using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Persistent.BaseImpl
{
    [NonPersistent]
    public abstract class eXpandBaseObject : BaseObject
    {
        public const string CancelTriggerObjectChangedName = "CancelTriggerObjectChanged";
//        private string skin;

        protected eXpandBaseObject(Session session)
            : base(session)
        {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public object IsNewObject
        {
            get { return Session.IsNewObject(this); }
        }

        protected override void OnSaving()
        {
            if (TrucateStrings)
                trucateStrings();
            base.OnSaving();
        }


        private void trucateStrings()
        {
            foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties)
            {
                if (xpMemberInfo.MemberType == typeof (string))
                {
                    var value = xpMemberInfo.GetValue(this) as string;
                    if (value != null)
                    {
                        //                            object[] attributes = xpMemberInfo.GetCustomAttributes(typeof (SizeAttribute),true);
                        if (xpMemberInfo.HasAttribute(typeof (SizeAttribute)))
                        {
                            int size = ((SizeAttribute) xpMemberInfo.GetAttributeInfo(typeof (SizeAttribute))).Size;
                            if (size > -1 && value.Length > size)
                                value = value.Substring(0, size - 1);
                        }
                        else if (value.Length > 99)
                            value = value.Substring(0, 99);
                        xpMemberInfo.SetValue(this, value);
                    }
                }
            }
        }

//        #region ISkinToModule Members
//        [Browsable(false)]
//        [Indexed]
//        public string Skin
//        {
//            get { return skin; }
//            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref skin, value); }
//        }
//        #endregion

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
            if (session.IsNewObject(xpBaseObject))
            {
                foreach (XPMemberInfo memberInfo in xpBaseObject.ClassInfo.ObjectProperties)
                {
                    if (memberInfo.HasAttribute(typeof (ExpandObjectMembersAttribute)))
                    {
                        if (
                            ((ExpandObjectMembersAttribute)
                             memberInfo.GetAttributeInfo(typeof (ExpandObjectMembersAttribute))).ExpandingMode !=
                            ExpandObjectMembers.Never)
                            memberInfo.SetValue(xpBaseObject,
                                                Activator.CreateInstance(memberInfo.MemberType,
                                                                         new object[] {xpBaseObject.Session}));
                    }
                }
            }
        }

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!CancelTriggerObjectChanged)
                base.TriggerObjectChanged(args);
        }

        [Browsable(false)]
        [NonPersistent]
        [MemberDesignTimeVisibility(false)]
        public bool CancelTriggerObjectChanged { get; set; }

        [Browsable(false)]
        [NonPersistent]
        [MemberDesignTimeVisibility(false)]
        public bool TrucateStrings { get; set; }
    }
}

