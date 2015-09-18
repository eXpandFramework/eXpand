using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Design;
using System.Security.Principal;
using DevExpress.Data.Filtering;
using Fasterflect;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [NonPersistent]
    [Serializable]
    public class AuthenticationCombinedLogonParameters : INotifyPropertyChanged, ICustomLogonParameter {
        private bool _useActiveDirectory;
        private string _userName;
        private string _password;

        public bool UseActiveDirectory {
            get { return _useActiveDirectory; }
            set { _useActiveDirectory = value; RaisePropertyChanged("UseActiveDirectory"); }
        }
        public string UserName {
            get { return _userName; }
            set { _userName = value; RaisePropertyChanged("UserName"); }
        }
        [ModelDefault("IsPassword", "True")]
        public string Password {
            get { return _password; }
            set { _password = value; RaisePropertyChanged("Password"); }
        }
        protected void RaisePropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabSecurity)]
    public class AuthenticationCombined : AuthenticationBase, IAuthenticationStandard {
        private AuthenticationCombinedLogonParameters _logonParameters;
        Type _userType;
        Type _logonParametersType;
        private bool _createUserAutomatically;
        public AuthenticationCombined() {
            LogonParametersType = typeof(AuthenticationCombinedLogonParameters);
        }
        public AuthenticationCombined(Type userType, Type logonParametersType) {
            _userType = userType;
            LogonParametersType = logonParametersType;
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            if (_logonParameters.UseActiveDirectory)
                return AuthenticateActiveDirectory(objectSpace);
            return AuthenticateStandard(objectSpace);
        }

        private object AuthenticateStandard(IObjectSpace objectSpace) {
            if (string.IsNullOrEmpty(_logonParameters.UserName))
                throw new ArgumentException(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.UserNameIsEmpty));
            var user = (IAuthenticationStandardUser)objectSpace.FindObject(UserType, new BinaryOperator("UserName", _logonParameters.UserName));
            if (user == null || !user.ComparePassword(_logonParameters.Password)) {
                throw new AuthenticationException(_logonParameters.UserName, SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
            }
            return user;
        }
        private object AuthenticateActiveDirectory(IObjectSpace objectSpace) {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null) {
                string userName = windowsIdentity.Name;
                object user = objectSpace.FindObject(UserType, new BinaryOperator("UserName", userName));
                if (user == null) {
                    if (_createUserAutomatically) {
                        CustomCreateUserEventArgs args = new CustomCreateUserEventArgs(objectSpace, userName);
                        if (CustomCreateUser != null)  {
                            CustomCreateUser(this, args);
                            user = (IAuthenticationActiveDirectoryUser)args.User;
                        }
                        if (!args.Handled) {
                            user = objectSpace.CreateObject(UserType);
                            ((IAuthenticationActiveDirectoryUser)user).UserName = userName;
                            if (Security != null)
                            {
                                //Security.InitializeNewUser(objectSpace, user);

                                // Make this a best-effort attempt to call InitializeNewUser - the ICanInitializeNewUser
                                // interface isn't accessable so we can't check if UserType actually implements it.
                                try { Security.CallMethod("InitializeNewUser", new object[] { objectSpace, user }); }
                                catch { }
                            }

                        }
                        bool strictSecurityStrategyBehavior = SecurityModule.StrictSecurityStrategyBehavior;
                        SecurityModule.StrictSecurityStrategyBehavior = false;
                        objectSpace.CommitChanges();
                        SecurityModule.StrictSecurityStrategyBehavior = strictSecurityStrategyBehavior;
                    }
                }

                if (user == null) {
                    throw new AuthenticationException(userName);
                }
                return user;
            }
            return null;
        }

        public override void ClearSecuredLogonParameters() {
            _logonParameters.Password = string.Empty;
            base.ClearSecuredLogonParameters();
        }
        public override bool IsSecurityMember(Type type, string memberName) {
            if (typeof(IAuthenticationStandardUser).IsAssignableFrom(type)) {
                if (typeof(IAuthenticationStandardUser).GetMember(memberName).Length > 0) {
                    return true;
                }
            }
            if (typeof(IAuthenticationActiveDirectoryUser).IsAssignableFrom(type)) {
                if (typeof(IAuthenticationActiveDirectoryUser).GetMember(memberName).Length > 0) {
                    return true;
                }
            }
            return false;
        }
        [Browsable(false)]
        public override object LogonParameters {
            get {
                return _logonParameters;
            }
        }
        public override bool AskLogonParametersViaUI {
            get {
                return true;
            }
        }
        public override bool IsLogoffEnabled {
            get { return true; }
        }
        public override IList<Type> GetBusinessClasses() {
            var result = new List<Type>();
            if (UserType != null) {
                result.Add(UserType);
            }
            if (LogonParametersType != null) {
                result.Add(LogonParametersType);
            }
            return result;
        }

        [Category("Behavior")]
        public override Type UserType {
            get { return _userType; }
            set {
                _userType = value;
                if (_userType != null && !typeof(IAuthenticationStandardUser).IsAssignableFrom(_userType)
                    && !typeof(IAuthenticationActiveDirectoryUser).IsAssignableFrom(_userType)) {
                    throw new ArgumentException(string.Format("AuthenticationCombined does not support the {0} user type.\nA class that implements the IAuthenticationStandardUser interface should be set for the UserType property.", _userType));
                }
            }
        }
        [TypeConverter(typeof(BusinessClassTypeConverter<AuthenticationCombinedLogonParameters>))]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Behavior")]
        public Type LogonParametersType {
            get { return _logonParametersType; }
            set {
                _logonParametersType = value;
                if (value != null) {
                    if (!typeof(AuthenticationCombinedLogonParameters).IsAssignableFrom(_logonParametersType)) {
                        throw new ArgumentException("LogonParametersType");
                    }
                    _logonParameters = (AuthenticationCombinedLogonParameters)_logonParametersType.CreateInstance(new object[0]);
                }
            }
        }
        [Category("Behavior")]
        public bool CreateUserAutomatically {
            get { return _createUserAutomatically; }
            set { _createUserAutomatically = value; }
        }

        public event EventHandler<CustomCreateUserEventArgs> CustomCreateUser;
    }
}