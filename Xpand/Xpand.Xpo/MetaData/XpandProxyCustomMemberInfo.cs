using System;
using System.ComponentModel;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData{
    public interface IXpandProxyCustomMemberInfo{
        event EventHandler<CustomXPMemberInfoValueEventArgs> CustomSetValue;
        event EventHandler<CustomXPMemberInfoValueEventArgs> CustomGetValue;
    }

    public class XpandProxyCustomMemberInfo : XpandCustomMemberInfo, IXpandProxyCustomMemberInfo{
        public XpandProxyCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType,
            XPClassInfo referenceType, bool nonPersistent, bool nonPublic)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic){
        }

        public XpandProxyCustomMemberInfo(XPClassInfo owner, string propertyName, Type propertyType,
            XPClassInfo referenceType, bool nonPersistent, bool nonPublic, bool readOnly)
            : base(owner, propertyName, propertyType, referenceType, nonPersistent, nonPublic, readOnly){
        }

        public event EventHandler<CustomXPMemberInfoValueEventArgs> CustomSetValue;
        public event EventHandler<CustomXPMemberInfoValueEventArgs> CustomGetValue;

        protected virtual void OnCustomSetValue(CustomXPMemberInfoValueEventArgs e){
            EventHandler<CustomXPMemberInfoValueEventArgs> handler = CustomSetValue;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnCustomGetValue(CustomXPMemberInfoValueEventArgs e){
            EventHandler<CustomXPMemberInfoValueEventArgs> handler = CustomGetValue;
            if (handler != null) handler(this, e);
        }

        public override object GetValue(object theObject){
            var args = new CustomXPMemberInfoValueEventArgs(theObject);
            OnCustomGetValue(args);
            if (!args.Handled)
                return base.GetValue(theObject);
            return args.Value;
        }

        public override void SetValue(object theObject, object theValue){
            var args = new CustomXPMemberInfoValueEventArgs(theObject, theValue);
            OnCustomSetValue(args);
            base.SetValue(theObject, theValue);
        }
    }

    public class CustomXPMemberInfoValueEventArgs : HandledEventArgs{
        private readonly object _currentObject;

        public CustomXPMemberInfoValueEventArgs(object currentObject){
            _currentObject = currentObject;
        }

        public CustomXPMemberInfoValueEventArgs(object currentObject, object value){
            Value = value;
            _currentObject = currentObject;
        }

        public object CurrentObject{
            get { return _currentObject; }
        }

        public object Value { get; set; }
    }
}