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

namespace SecurityDemo.Module {
    [NonPersistent, Hint(Hints.LogonWindowHeaderHint, ViewType.DetailView, "UserDescription")]
    [Serializable]
    [System.ComponentModel.DisplayName("Log On")]
    public class SecurityDemoAuthenticationLogonParameters : INotifyPropertyChanged, ISerializable {
        private IObjectSpace objectSpace;
        private XPCollection<SecurityDemoUser> availableUsers;
        private SecurityDemoUser user;

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
        public XPCollection<SecurityDemoUser> AvailableUsers {
            get {
                if(availableUsers == null) {

                    availableUsers = (XPCollection<SecurityDemoUser>)ObjectSpace.GetObjects<SecurityDemoUser>();
                    availableUsers.BindingBehavior = CollectionBindingBehavior.AllowNone;
                }
                return availableUsers;
            }
        }
        [Browsable(false)]
        public string UserDescription {
            get {
                if(User != null) {
                    return User.Description;
                }
                return "";
            }
        }
        [DataSourceProperty("AvailableUsers"), ImmediatePostData]
        public SecurityDemoUser User {
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
                OnPropertyChanged("UserDescription");
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

    public class SecurityDemoAuthentication : AuthenticationBase {
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
            object user = objectSpace.FindObject<SecurityDemoUser>(new DevExpress.Data.Filtering.BinaryOperator("UserName", logonParameters.UserName));
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
