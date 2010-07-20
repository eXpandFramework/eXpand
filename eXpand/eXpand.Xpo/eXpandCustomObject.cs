using System;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo
{
    [Serializable]
    [NonPersistent]
    public abstract class eXpandCustomObject : XPCustomObject, ISupportChangedMembers
    {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
        [Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
        private Guid oid = Guid.Empty;
        [PersistentAlias("oid"), Browsable(false)]
        public Guid Oid
        {
            get { return oid; }
        }
#endif
        private bool isDefaultPropertyAttributeInit;
        private XPMemberInfo defaultPropertyMemberInfo;
        ChangedMemberCollector _changedMemberCollector;
//        private MemberInfoCollection _changedMembers;

//        protected override void OnChanged(string propertyName, object oldValue, object newValue)
//        {
//            base.OnChanged(propertyName, oldValue, newValue);
//
//            if (!IsLoading)
//            {
//                XPMemberInfo member = GetPersistentMember(propertyName);
//                if (member != null && !ChangedMembers.Contains(member))
//                {
//                    _changedMembers.Add(ClassInfo.GetMember(member.Name));
//                }
//            }
//        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (TrucateStrings)
                trucateStrings();
            if (!(Session is NestedUnitOfWork) && Session.IsNewObject(this) && oid == Guid.Empty)
            {
                oid = XpoDefault.NewGuid();
            }
        }

//        protected override void OnSaved()
//        {
//            base.OnSaved();
//
//            if (Session is NestedUnitOfWork)
//            {
//                var parentitem = ((NestedUnitOfWork)Session).GetParentObject(this);
//                foreach (XPMemberInfo changedProperty in ChangedMembers)
//                {
//                    if (!parentitem.ChangedMembers.Contains(changedProperty))
//                    {
//                        parentitem.ChangedMembers.Add(changedProperty);
//                    }
//                }
//            }
//
//            ChangedMembers.Clear();
//        }

        public override string ToString()
        {
            if (!isDefaultPropertyAttributeInit)
            {
                var attrib = ClassInfo.FindAttributeInfo(typeof(DefaultPropertyAttribute)) as DefaultPropertyAttribute;
                if (attrib != null)
                {
                    defaultPropertyMemberInfo = ClassInfo.FindMember(attrib.Name);
                }
                isDefaultPropertyAttributeInit = true;
            }
            if (defaultPropertyMemberInfo != null)
            {
                object obj = defaultPropertyMemberInfo.GetValue(this);
                if (obj != null)
                {
                    return obj.ToString();
                }
            }
            return base.ToString();
        }

//        private XPMemberInfo GetPersistentMember(string propertyName)
//        {
//            XPMemberInfo persistentMember = ClassInfo.GetPersistentMember(propertyName);
//            if (persistentMember == null)
//            {
//                var memberInfo = ClassInfo.FindMember(propertyName);
//                if (memberInfo != null && memberInfo.IsAliased)
//                {
//                    var pa = (PersistentAliasAttribute)memberInfo.GetAttributeInfo(typeof(PersistentAliasAttribute));
//                    CriteriaOperator criteria = CriteriaOperator.Parse(pa.AliasExpression);
//                    if (criteria is OperandProperty){
//                        string[] path = ((OperandProperty)criteria).PropertyName.Split('.');
//                        return path.Aggregate<string, XPMemberInfo>(null, (current, pn) => current == null ? ClassInfo.GetMember(pn) : current.ReferenceType.GetMember(pn));
//                    }
//                }
//            }
//
//            return persistentMember;
//        }

        public const string CancelTriggerObjectChangedName = "CancelTriggerObjectChanged";
        //        protected eXpandCustomObject() {}
        protected eXpandCustomObject(Session session) : base(session) { }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public bool IsNewObject
        {
            get { return Session.IsNewObject(this); }
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

        private void trucateStrings()
        {
            foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties)
            {
                if (xpMemberInfo.MemberType == typeof(string))
                {
                    var value = xpMemberInfo.GetValue(this) as string;
                    if (value != null)
                    {
                        value = TruncateValue(xpMemberInfo, value);
                        xpMemberInfo.SetValue(this, value);
                    }
                }
            }
        }

        string TruncateValue(XPMemberInfo xpMemberInfo, string value)
        {
            if (xpMemberInfo.HasAttribute(typeof(SizeAttribute)))
            {
                int size = ((SizeAttribute)xpMemberInfo.GetAttributeInfo(typeof(SizeAttribute))).Size;
                if (size > -1 && value.Length > size)
                    value = value.Substring(0, size - 1);
            }
            else if (value.Length > 99)
                value = value.Substring(0, 99);
            return value;
        }

        #region ISupportChangedMembers Member

//        [Browsable(false)]
//        public MemberInfoCollection ChangedMembers
//        {
//            get { return _changedMembers ?? (_changedMembers = new MemberInfoCollection(ClassInfo)); }
//        }

        #endregion
        [Browsable(false)]
        public ChangedMemberCollector ChangedMemberCollector {
            get { return _changedMemberCollector ?? (_changedMemberCollector = new ChangedMemberCollector(this)); }
        }
    }
}