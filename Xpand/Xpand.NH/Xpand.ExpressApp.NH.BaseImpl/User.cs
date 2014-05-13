using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    [DataContract]
    [DefaultClassOptions]
    public class User : ISecurityUser, ISecurityUserWithRoles, IAuthenticationStandardUser
    {

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public bool IsActive
        {
            get;
            set;
        }

        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        [DataMember]
        public IList<ISecurityRole> Roles
        {
            get { return null; }
        }

        [DataMember]
        public bool ChangePasswordOnFirstLogon
        {
            get;
            set;
        }

        [DataMember]
        public string StoredPassword
        {
            get;
            set;
        }

        public bool ComparePassword(string password)
        {
            return new PasswordCryptographer().AreEqual(StoredPassword, password);
        }

        public static string GeneratePassword(string password)
        {
            return new PasswordCryptographer().GenerateSaltedPassword(password);
        }
        public void SetPassword(string password)
        {
            StoredPassword = GeneratePassword(password);
        }
    }
}