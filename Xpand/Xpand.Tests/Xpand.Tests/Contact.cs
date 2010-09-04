using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Tests;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using System.Linq;


namespace Fixtures
{
    public class Contact
    {
        private string firstName;
        public virtual string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        protected virtual void OnSettingUp(ExpressApplicationSetupParameters parameters)
        {
            
        }
//        private string lastName;
//        public virtual string LastName
//        {
//            get { return lastName; }
//            set { lastName = value; }
//        }
//
//        private string emailAddress;
//        public virtual string EmailAddress
//        {
//            get { return emailAddress; }
//            set { emailAddress = value; }
//        }
    }
}