using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace SecurityDemo.Module {
    [NonPersistent, Hint(Hints.LogonWindowHeaderHint, ViewType.DetailView)]
    [Serializable]
    [System.ComponentModel.DisplayName("Log On")]
    public class SecurityDemoAuthenticationLogonParameters : INotifyPropertyChanged, ISerializable {
        private IObjectSpace objectSpace;
        private XPCollection<PermissionPolicyUser> availableUsers;
        private PermissionPolicyUser user;

        public SecurityDemoAuthenticationLogonParameters() { }
        public SecurityDemoAuthenticationLogonParameters(SerializationInfo info, StreamingContext context) {
            if(info.MemberCount > 0) {
                UserName = info.GetString("UserName");
            }
        }

        [Browsable(false)]
        public IObjectSpace ObjectSpace {
            get { return objectSpace; }
            set { objectSpace = value; }
        }
        [Browsable(false)]
        public XPCollection<PermissionPolicyUser> AvailableUsers {
            get {
                if(availableUsers == null) {

                    availableUsers = (XPCollection<PermissionPolicyUser>)ObjectSpace.GetObjects<PermissionPolicyUser>();
                    availableUsers.BindingBehavior = CollectionBindingBehavior.AllowNone;
                }
                return availableUsers;
            }
        }
        [DataSourceProperty("AvailableUsers"), ImmediatePostData]
        public PermissionPolicyUser User {
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

        private void OnPropertyChanged(string propertyName) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("UserName", UserName);
        }
        [Browsable(false)]
        public string UserName { get; set; }
    }

    public class SecurityDemoAuthentication : AuthenticationBase, IAuthenticationStandard {
        private SecurityDemoAuthenticationLogonParameters logonParameters;

        public SecurityDemoAuthentication() {
            logonParameters = new SecurityDemoAuthenticationLogonParameters();
        }
        public override void Logoff() {
            base.Logoff();
            logonParameters = new SecurityDemoAuthenticationLogonParameters();
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            if(string.IsNullOrEmpty(logonParameters.UserName)) {
                throw new UserFriendlyException("The 'User' field must not be empty.");
            }
            object user = objectSpace.FindObject<PermissionPolicyUser>(new DevExpress.Data.Filtering.BinaryOperator("UserName", logonParameters.UserName));
            if(user == null) {
                throw new AuthenticationException(logonParameters.UserName);
            }
            return user;
        }
        public override IList<Type> GetBusinessClasses() {
            return new Type[] { typeof(SecurityDemoAuthenticationLogonParameters) };
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
