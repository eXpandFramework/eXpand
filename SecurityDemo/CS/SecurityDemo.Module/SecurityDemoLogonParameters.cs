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
    public class SecurityDemoAuthenticationLogonParameters : INotifyPropertyChanged, ISupportResetLogonParameters, ISerializable {
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

                    availableUsers = (XPCollection<SecurityDemoUser>)ObjectSpace.GetObjects<SecurityDemoUser>(new NotOperator(new BinaryOperator("UserName", SecurityStrategy.AnonymousUserName)));
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

        public void Reset() {
            objectSpace = null;
            availableUsers = null;
            user = null;
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
            KnownTypesProvider.KnownTypes.Add(typeof(SecurityDemoAuthenticationLogonParameters));
            logonParameters = new SecurityDemoAuthenticationLogonParameters();
        }

        public override object GetDefaultLogonParameters() {
            SecurityDemoAuthenticationLogonParameters result = new SecurityDemoAuthenticationLogonParameters();
            result.UserName = SecurityStrategy.AnonymousUserName;
            return result;
        }
        public override object Authenticate(object logonParameters, IObjectSpace objectSpace) {
            SecurityDemoAuthenticationLogonParameters securityDemoAuthenticationLogonParameters = logonParameters as SecurityDemoAuthenticationLogonParameters;
            if(string.IsNullOrEmpty(securityDemoAuthenticationLogonParameters.UserName)) {
                throw new ArgumentNullException("User");
            }
            return objectSpace.FindObject<SecurityDemoUser>(new DevExpress.Data.Filtering.BinaryOperator("UserName", securityDemoAuthenticationLogonParameters.UserName));
            //return objectSpace.GetObject(securityDemoAuthenticationLogonParameters.User);
        }
        public override object Authenticate(DevExpress.ExpressApp.IObjectSpace objectSpace) {
            return Authenticate(LogonParameters, objectSpace);
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
