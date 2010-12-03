using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultProperty("Name")]
    public abstract class PersistentTypeInfo : XpandBaseCustomObject, IPersistentTypeInfo {
        public event EventHandler<ObjectCreatedEventArgs> AfterConstructed;

        public void InvokeAfterConstructed(ObjectCreatedEventArgs e) {
            EventHandler<ObjectCreatedEventArgs> handler = AfterConstructed;
            if (handler != null) handler(this, e);
        }

        string _name;
        public override void AfterConstruction() {
            base.AfterConstruction();
            InvokeAfterConstructed(new ObjectCreatedEventArgs(this, null));
        }
        protected PersistentTypeInfo(Session session)
            : base(session) {
        }


        [Association("TypeAttributes")]
        [Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }

        IList<ITemplateInfo> IPersistentTypeInfo.TemplateInfos {
            get { return new ListConverter<ITemplateInfo, TemplateInfo>(TemplateInfos); }
        }

        [Association("PersistentTypeInfo-TemplateInfos")]
        public XPCollection<TemplateInfo> TemplateInfos {
            get { return GetCollection<TemplateInfo>("TemplateInfos"); }
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


        #endregion
    }

}