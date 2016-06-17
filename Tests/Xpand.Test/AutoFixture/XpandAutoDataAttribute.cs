using System;
using System.Linq;
using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Xpand.Test.AutoFixture.Customizations;
using Xpand.Utils.Helpers;

namespace Xpand.Test.AutoFixture{
    public class XpandAutoDataAttribute : AutoDataAttribute{
        public XpandAutoDataAttribute(params Type[] types){
            types.Each(type => Fixture.Customize((ICustomization)Activator.CreateInstance(type)));
            Fixture.Customizations.Add(new TypeRelay(typeof(ITestData),typeof(TestData)));
            Fixture.Customize(new AutoMoqCustomization());
            Fixture.Customize(new TestXafApplicationCustomization());
            Fixture.Customize(new ObjectSpaceCustomization());
            Fixture.Customize(new PersistentObjectParameterCustomization());
            Fixture.Customize(new ModuleBaseCustomization());
            Fixture.Customize(new WindowCustomization());
        }

        protected static Type[] GetTypes<T>(Type[] types) where T:ModuleBase{
            return new[] { typeof(ModuleBaseCustomization<T>) }.Concat(types).ToArray();
        }

        public int RepeatCount{
            get { return Fixture.RepeatCount; }
            set { Fixture.RepeatCount = value; }
        }
    }
}
