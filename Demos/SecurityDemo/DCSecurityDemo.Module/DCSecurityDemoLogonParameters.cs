using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DCSecurityDemo.Module.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using SecurityDemo.Module;

namespace DCSecurityDemo.Module {
    [DomainComponent]
    [Serializable]
    [XafDisplayName("Log On")]
    [Hint(Hints.LogonWindowHeaderHint, ViewType.DetailView)]
    public class DCSecurityDemoAuthenticationLogonParameters : INotifyPropertyChanged, ISerializable {
        private IObjectSpace objectSpace;
        private IList<IDCUser> availableUsers;
        private IDCUser user;

        private void OnPropertyChanged(String propertyName) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DCSecurityDemoAuthenticationLogonParameters() { }
        public DCSecurityDemoAuthenticationLogonParameters(SerializationInfo info, StreamingContext context) {
            if(info.MemberCount > 0) {
                UserName = info.GetString("UserName");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("UserName", UserName);
        }

        [Browsable(false)]
        public IObjectSpace ObjectSpace {
            get { return objectSpace; }
            set { objectSpace = value; }
        }
        [Browsable(false)]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public IList<IDCUser> AvailableUsers {
            get {
                if(availableUsers == null) {
                    availableUsers = ObjectSpace.GetObjects<IDCUser>();
                }
                return availableUsers;
            }
        }
        [Browsable(false)]
        public String UserName { get; set; }
        [DataSourceProperty("AvailableUsers"), ImmediatePostData]
        public IDCUser User {
            get { return user; }
            set {
                user = value;
                if(user != null) {
                    UserName = user.UserName;
                }
                else {
                    UserName = "";
                }
                OnPropertyChanged("User");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class DCSecurityDemoAuthentication : AuthenticationBase, IAuthenticationStandard {
        private DCSecurityDemoAuthenticationLogonParameters logonParameters;

        public DCSecurityDemoAuthentication() {
            logonParameters = new DCSecurityDemoAuthenticationLogonParameters();
        }
        public override void Logoff() {
            base.Logoff();
            logonParameters = new DCSecurityDemoAuthenticationLogonParameters();
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            if(String.IsNullOrEmpty(logonParameters.UserName)) {
                throw new UserFriendlyException("The 'User' field must not be empty.");
            }
            object user = objectSpace.FindObject<IDCUser>(new BinaryOperator("UserName", logonParameters.UserName));
            if(user == null) {
                throw new AuthenticationException(logonParameters.UserName);
            }
            return user;
        }
        public override IList<Type> GetBusinessClasses() {
            return new Type[] { typeof(DCSecurityDemoAuthenticationLogonParameters) };
        }
        public override bool AskLogonParametersViaUI {
            get { return true; }
        }
        public override object LogonParameters {
            get { return logonParameters; }
        }
        public override bool IsLogoffEnabled {
            get { return true; }
        }
    }
}
