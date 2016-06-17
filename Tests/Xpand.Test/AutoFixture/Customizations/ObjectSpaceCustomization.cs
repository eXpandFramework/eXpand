using System.Data;
using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Xpand.Persistent.Base.General;

namespace Xpand.Test.AutoFixture.Customizations{
    public class ObjectSpaceCustomization : ICustomization{
        public void Customize(IFixture fixture){
            fixture.Inject(new DataSet());
            var objectSpace = ObjectSpaceInMemory.CreateNew(fixture.Create<DataSet>());
            fixture.Inject(objectSpace);
            fixture.Inject(objectSpace.Session());
        }
    }
}