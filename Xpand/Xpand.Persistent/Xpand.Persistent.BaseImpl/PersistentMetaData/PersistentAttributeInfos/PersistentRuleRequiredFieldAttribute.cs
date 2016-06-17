using System;
using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos{
    [InterfaceRegistrator(typeof(IPersistentRuleRequiredFieldAttribute))]
    [DefaultProperty("DefaultProperty")]
    [System.ComponentModel.DisplayName("Rule Required")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentRuleRequiredFieldAttribute : PersistentAttributeInfo, IPersistentRuleRequiredFieldAttribute{
        private string _context;
        private string _iD;
        private string _targetCriteria;


        public PersistentRuleRequiredFieldAttribute(Session session)
            : base(session){
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string DefaultProperty => $"{typeof(RuleRequiredFieldAttribute).Name}: {ID}";

        [Size(SizeAttribute.Unlimited)]
        [AttributeInfo]
        public string TargetCriteria{
            get { return _targetCriteria; }
            set { SetPropertyValue("TargetCriteria", ref _targetCriteria, value); }
        }

        public string ID{
            get { return _iD; }
            set { SetPropertyValue("ID", ref _iD, value); }
        }

        [RuleRequiredField]
        public string Context{
            get { return _context; }
            set { SetPropertyValue("Context", ref _context, value); }
        }

        public override AttributeInfoAttribute Create(){
            if (string.IsNullOrEmpty(ID)&&string.IsNullOrEmpty(Context))
                return new AttributeInfoAttribute(
                        typeof(RuleRequiredFieldAttribute).GetConstructor(Type.EmptyTypes)) { Instance = this };
            return new AttributeInfoAttribute(
                    typeof(RuleRequiredFieldAttribute).GetConstructor(new[] { typeof(string),typeof(string) }), ID,Context) { Instance = this };
        }
    }
}