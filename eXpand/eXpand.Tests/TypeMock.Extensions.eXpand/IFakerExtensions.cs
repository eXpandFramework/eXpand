using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using TypeMock.ArrangeActAssert;

namespace TypeMock.Extensions.eXpand{
    public static class IFakerExtensions
    {
        public static ISecurityComplex ISecurityComplex(this IFaker faker) 
        {
            var securityComplex = Isolate.Fake.Instance<ISecurityComplex>();
            Isolate.WhenCalled(() => SecuritySystem.Instance).WillReturn(securityComplex);
            Isolate.WhenCalled(() => securityComplex.RoleType).WillReturn(typeof(Role));
            Isolate.WhenCalled(() => securityComplex.UserType).WillReturn(typeof(User));

            Isolate.Fake.StaticMethods(typeof(SecuritySystem));
            Isolate.WhenCalled(() => SecuritySystem.UserType).WillReturn(typeof(User));

            var user = new User(Session.DefaultSession);
            user.Save();
            Isolate.WhenCalled(() => SecuritySystem.CurrentUser).WillReturn(user);

            XafTypesInfo.Instance.RegisterEntity(securityComplex.RoleType);
            XafTypesInfo.Instance.RegisterEntity(securityComplex.UserType);
            return securityComplex;
        }
    }
}