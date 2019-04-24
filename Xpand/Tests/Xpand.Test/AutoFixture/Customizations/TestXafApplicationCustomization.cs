using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Xpand.Test.AutoFixture.Customizations {
    public class TestXafApplicationCustomization : ICustomization {
        public void Customize(IFixture fixture){
            fixture.Customizations.Add(new TypeRelay(typeof(XafApplication), typeof(TestXafApplication)));
            fixture.Customizations.Add(new TypeRelay(typeof(LayoutManager), typeof(TestLayoutManager)));
            fixture.Customize<TestXafApplication>(composer => composer.OmitAutoProperties());
            fixture.Freeze<TestXafApplication>();
        }
    }

}
