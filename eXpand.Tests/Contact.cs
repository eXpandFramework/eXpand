using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo.Metadata;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

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