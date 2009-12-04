using System.Collections.Generic;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo {
        string _generatedCode;
        string _name;

        protected PersistentTypeInfo(Session session) : base(session) {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            TemplateInfo=new TemplateInfo(Session);
        }

        [Association("TypeAttributes")]
        [Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }
        #region IPersistentTypeInfo Members
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        IList<IPersistentAttributeInfo> IPersistentTypeInfo.TypeAttributes {
            get { return new ListConverter<IPersistentAttributeInfo, PersistentAttributeInfo>(TypeAttributes); }
        }

        [Size(SizeAttribute.Unlimited)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        public string GeneratedCode {
            get { return _generatedCode; }
            set { SetPropertyValue("GeneratedCode", ref _generatedCode, value); }
        }
        private CodeTemplate _codeTemplate;
        [RuleRequiredField(null,DefaultContexts.Save)]
        public CodeTemplate CodeTemplate
        {
            get
            {
                return _codeTemplate;
            }
            set
            {
                SetPropertyValue("CodeTemplate", ref _codeTemplate, value);
            }
        }
        private TemplateInfo _templateInfo;

        [VisibleInListView(false)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Aggregated]
        [Association("TemplateInfo-PersistentTypeInfos")]
        public TemplateInfo TemplateInfo
        {
            get
            {
                return _templateInfo;
            }
            set
            {
                SetPropertyValue("TemplateInfo", ref _templateInfo, value);
            }
        }
        ITemplateInfo IPersistentTypeInfo.TemplateInfo {
            get { return TemplateInfo; }
            set { TemplateInfo=value as TemplateInfo; }
        }

        ICodeTemplate IPersistentTypeInfo.CodeTemplate {
            get { return CodeTemplate; }
            set { CodeTemplate=value as CodeTemplate; }
        }

//        ITemplateInfo IPersistentTypeInfo.TemplateInfo {
//            get { return TemplateInfo; }
//            set { TemplateInfo = value as TemplateInfo; }
//        }
        #endregion
    }

}